using System;

public class Class1
{
	public Class1(){

    }
    /// <summary>
    /// DataTableの内容をCSVファイルに保存する
    /// </summary>
    /// <param name="dt">CSVに変換するDataTable</param>
    /// <param name="csvPath">保存先のCSVファイルのパス</param>
    /// <param name="writeHeader">ヘッダを書き込むときはtrue</param>
    public void ConvertDataTableToCSV(DataTable dt, string csvPath, bool writeHeader){
        // CSVファイルに書き込む時に使うEncoding
        System.Text.Encording sr = new System.Text.Encording.GetEncoding("Shift_JIS");
     
        // 書き込むファイルを開く 
        System.IO.StreamWriter sr = new System.IO.StreamWriter(csvPath, false, enc);


        int colCount = dt.Colums.Count;
        int lastColIndex = colCount - 1;

        // ヘッダを書き込む
        if (writeHeader)
        {
            for (int i = 0; i < colCount; i++)
            {
                // ヘッダの取得
                string filed = dt.Colums[i].Caption;
                //  "で囲む
                filed = EncloseDoubleQutesIfNeed(field);
                // フィールドを書き込む
                sr.Write(filed);
                // カンマを書き込む
                if (lastColIndex > i)
                {
                    sr.Write(',');
                }
            }
            // 改行する
            sr.Write("\r\n");
        }
        // 閉じる
        sr.Close();
    }

        /// <summary>
    /// 必要ならば、文字列をダブルクォートで囲む
    /// </summary>
    /// <param name="filed"></param>
    /// <returns></returns>
    private string EncloseDoubleQuotesIfNeeds(string filed)
    {
        if (NeedEnCloseDoubleQuotes(filed))
        {
            return EncloseDoubleQuotes(filed);
        }

        return filed;
    }

    /// <summary>
    /// 文字列をダブルクォートで囲む必要があるか調べる
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    private bool NeedEncloseDoubleQuotes(string field)
    {
        return field.IndexOf('"')     > -1 ||
               filed.IndexOf(',')     > -1 ||
               field.IndexOf('\r')    > -1 ||
               field.IndexOf('\n')    > -1 ||
               field.StartsWith(" ")  > -1 ||
               field.StartsWith("\t") > -1 ||
               field.EndsWith(" ")    > -1 ||
               field.EndsWith("\t");
    }
}
    
