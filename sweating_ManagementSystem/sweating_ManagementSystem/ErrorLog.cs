using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sweating_ManagementSystem
{
    class ErrorLog
    {/// <summary>
        /// エラーの発生した場所
        /// </summary>
        public enum ErrorLocation : int
        {
            Def,
            Login,
            Menu,
            BrushSearch,
            Guidance,
            Print,
            BaseMake,
            BaseSelect,
            UserAppend,
            Mentenance,
            ClaudSetting,
            NetworkSetting,
            HospitalSetting,
            Help,
            Analysis,
            UserSearch
        }

        /// <summary>
        /// 発生場所毎の出力文字列
        /// </summary>
        private String[] ErrorLocations;

        /// <summary>
        /// エラーログの最大行数
        /// </summary>
        private const int LogLineLength = 100;

        /// <summary>
        /// エラーログの出力先
        /// </summary>
        String path = new CCP("none").exePath + @"\log.txt";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ErrorLog()
        {
            int LocationCount = Enum.GetNames(typeof(ErrorLocation)).Length;
            ErrorLocations = new String[LocationCount];
            Init(LocationCount);
        }

        /// <summary>
        /// エラー場所の初期化
        /// </summary>
        /// <param name="cnt">ErrorLocationの数</param>
        private void Init(int cnt)
        {
            String text = "";
            for (int i = 0; i < cnt; i++)
            {
                switch ((ErrorLocation)i)
                {
                    case ErrorLocation.Analysis:
                        text = "データ送信　：　";
                        break;
                    case ErrorLocation.Login:
                        text = "データ取得　：　";
                        break;
                    default:
                        text = "UnknownException　：　";
                        break;
                }
                ErrorLocations[i] = text;
            }
        }

        /// <summary>
        /// ログの出力
        /// </summary>
        /// <param name="value">エラー表現文字列</param>
        /// <param name="location">発生場所</param>
        public void ErrorOutput(String value, ErrorLocation location)
        {
            String errorlog = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss　===　");
            errorlog += ErrorLocations[(int)location];
            errorlog += value + Environment.NewLine;
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.Create(path).Close();
            }
            string[] lines = System.IO.File.ReadAllLines(path);
            if (lines.Length >= LogLineLength)
            {
                lines = lines.Skip(1).ToArray();
                System.IO.File.WriteAllLines(path, lines);
            }
            System.IO.File.AppendAllText(path, errorlog);
        }
    }
}
