using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiApp1.Models
{

        [Table("vocabulary")]
        public class Vocabulary
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }

            [Indexed] // 單字搜尋
            public string Word { get; set; } // 英文單字

            public string PartOfSpeech { get; set; } // 詞性

            public string Definition { get; set; } // 中文定義

            public int LetterCount { get; set; } // 字母數

            public bool IsLearned { get; set; } = false; // 追蹤使用者是否學會了這個單字

            public int MistakeCount { get; set; } = 0; // 紀錄答錯次數


        /// <summary>
        /// SQLite 會忽略這個欄位，它只用於在單次測驗中暫存使用者的答案。
        /// </summary>
        [Ignore]
            public string UserAnswer { get; set; }

            /// <summary>
            /// SQLite 會忽略這個欄位，用來標記使用者是否答對了這一題。
            /// </summary>
            [Ignore]
            public bool IsCorrect { get; set; }
    }

}
