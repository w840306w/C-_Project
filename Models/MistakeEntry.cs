using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiApp1.Models;

[Table("mistakes")]
public class MistakeEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // 外鍵關聯到 Vocabulary 資料表
    [Indexed]
    public int VocabularyId { get; set; }

    // 記錄答錯的時間
    public DateTime MistakeDate { get; set; }
}