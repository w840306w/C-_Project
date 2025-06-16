using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp1.Models;
using SQLite;
using System.Text.Json;
namespace MauiApp1.Resources.DB
{
    
        public class DatabaseService
        {
            private SQLiteAsyncConnection _database;
            private bool _isInitialized = false;

            private static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, "MyWords.db3"); 

            public DatabaseService() { }

            /// <summary>
            /// 初始化方法，用來建立連線和資料表
            /// </summary>
            private async Task InitializeAsync()
            {
                if (_isInitialized)
                    return;

                _database = new SQLiteAsyncConnection(DatabasePath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);              
                _isInitialized = true;
                await _database.CreateTableAsync<Vocabulary>();
                await _database.CreateTableAsync<MistakeEntry>();

            }


            public async Task<int> GetTotalVocabularyCountAsync()
            {
                await InitializeAsync();
                return await _database.Table<Vocabulary>().CountAsync();
            }

            public async Task<List<Vocabulary>> GetVocabulariesAsync()
            {
                await InitializeAsync(); 
                return await _database.Table<Vocabulary>().ToListAsync();
            }

            public async Task SeedDataFromFileAsync()
            {
                await InitializeAsync();

                if (await GetTotalVocabularyCountAsync() > 0)
                {
                    // 如果資料庫已經有資料，就直接返回
                    return;
                }

                //獲取資料庫路徑
                //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyWords.db3");
                //var bytes = File.ReadAllBytes(path);

                //將資料庫檔案寫出
                //var fileCopyName = string.Format("/sdcard/Database_{0:dd-MM-yyyy_HH-mm-ss-tt}.db", DateTime.Now);
                //File.WriteAllBytes(fileCopyName, bytes);


                try
                {
                    // 嘗試讀取檔案
                    using var stream = await FileSystem.Current.OpenAppPackageFileAsync("eng1.json");
                    using var reader = new StreamReader(stream);
                    var jsonContent = await reader.ReadToEndAsync();

                    await MainThread.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("偵錯點 1", "成功讀取 JSON 檔案。", "OK"));

                    // 嘗試解析 JSON
                    var wordsFromFile = JsonSerializer.Deserialize<List<JsonWordItem>>(jsonContent);

                    await MainThread.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("偵錯點 2", $"成功解析 {wordsFromFile?.Count ?? 0} 個單字項目。", "OK"));

                    if (wordsFromFile != null)
                    {
                        var vocabulariesToInsert = new List<Vocabulary>();
                        foreach (var item in wordsFromFile)
                        {
                            foreach (var def in item.definitions)
                            {
                                vocabulariesToInsert.Add(new Vocabulary
                                {
                                    Word = item.word,
                                    LetterCount = item.letterCount,
                                    Definition = def.text,
                                    PartOfSpeech = def.partOfSpeech
                                });
                            }
                        }

                        // 嘗試寫入資料庫
                        await _database.InsertAllAsync(vocabulariesToInsert);

                        // 資料庫寫入成功
                        await MainThread.InvokeOnMainThreadAsync(async () => await Application.Current.MainPage.DisplayAlert("偵錯點 3", $"成功將 {vocabulariesToInsert.Count} 筆資料寫入資料庫。", "OK"));


                    }
                }
                catch (Exception ex)
                {

                    await MainThread.InvokeOnMainThreadAsync(async () => {
                        await Application.Current.MainPage.DisplayAlert("發生錯誤！", $"讀取初始資料時失敗: {ex.Message}", "OK");
                    });
                }
            }

            /// <summary>
            /// 更新一個單字的資料 (例如更新答錯次數)
            /// </summary>
            public async Task UpdateVocabularyAsync(Vocabulary word)
            {
                await InitializeAsync();
                await _database.UpdateAsync(word);
            }


            public async Task<List<Vocabulary>> GetMistakeVocabulariesAsync()
            {
                await InitializeAsync();

                // 修改 SQL 查詢，主要依據 MistakeCount 降冪排序，次要依據答錯日期降冪排序
                return await _database.QueryAsync<Vocabulary>(@"
                    SELECT V.* FROM Vocabulary V
                    INNER JOIN mistakes M ON V.ID = M.VocabularyId
                    ORDER BY V.MistakeCount DESC, V.Word ASC");
            }


            /// <summary>
            /// 新增一筆錯題紀錄
            /// </summary>
            public async Task AddMistakeAsync(Vocabulary word)
            {
                await InitializeAsync();

                // 為了避免重複記錄，可以先檢查是否已存在
                var existing = await _database.Table<MistakeEntry>()
                                              .Where(m => m.VocabularyId == word.ID)
                                              .FirstOrDefaultAsync();

                if (existing == null)
                {
                    var newMistake = new MistakeEntry
                    {
                        VocabularyId = word.ID,
                        MistakeDate = DateTime.Now
                    };
                    await _database.InsertAsync(newMistake);
                }
            }

            /// <summary>
            /// 清除所有錯題紀錄
            /// </summary>
            public async Task ClearAllMistakesAsync()
            {
                await InitializeAsync();
                await _database.DeleteAllAsync<MistakeEntry>();
            }

        }

        /// <summary>
        /// 這個類別用來匹配 JSON 檔案中，每個單字物件的結構
        /// </summary> 
        public class JsonWordItem
        {
            public int letterCount { get; set; }
            public string word { get; set; }
            public List<DefinitionItem> definitions { get; set; }
        }

        /// <summary>
        /// 這個類別用來匹配 JSON 檔案中，definitions 陣列裡每個定義物件的結構
        /// </summary>
        public class DefinitionItem
        {
            public string text { get; set; }
            public string partOfSpeech { get; set; }
        }

}