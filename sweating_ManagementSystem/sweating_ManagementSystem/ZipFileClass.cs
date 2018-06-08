using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Runtime.Serialization.Json;

namespace MOutHSystemApplication
{
    class ZipFileClass
    {
        public ZipFileClass()
        {

        }

        public System.IO.MemoryStream MakeZip(sweating_ManagementSystem.SweatingWeb.API_ANALYSIS_DATA json)
        {
            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(typeof(sweating_ManagementSystem.SweatingWeb.API_ANALYSIS_DATA));
            System.IO.MemoryStream mst = new System.IO.MemoryStream();
            jsonSer.WriteObject(mst, json);
            mst.Position = 0;
            using (System.IO.FileStream w = new System.IO.FileStream(System.Environment.CurrentDirectory + @"\analysis\jsonSJIS.txt", System.IO.FileMode.Create))
            {
                jsonSer.WriteObject(w, json);
            }
            using (System.IO.StreamReader r = new System.IO.StreamReader(System.Environment.CurrentDirectory + @"\analysis\jsonSJIS.txt"))
            {
                String s = r.ReadToEnd();
                using (System.IO.StreamWriter _w = new System.IO.StreamWriter(System.Environment.CurrentDirectory + @"\analysis\json.txt", false, new UTF8Encoding(false)))
                {
                    _w.Write(s);
                }
            }
            System.IO.File.Delete(System.Environment.CurrentDirectory + @"\analysis\jsonSJIS.txt");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //using(System.IO.FileStream fs = new System.IO.FileStream(@"C:\MOutHSystemApplication\analysis\json.zip", System.IO.FileMode.Open))
            //{
            //    byte[] b = new byte[fs.Length];
            //    fs.Read(b, 0, b.Length);
            //    ms.Write(b, 0, b.Length);
            //}
            using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, false))
            {
                var entry = zipArchive.CreateEntryFromFile(System.Environment.CurrentDirectory + @"\analysis\json.txt", "json.txt");
            }
            System.IO.File.Delete(System.Environment.CurrentDirectory + @"\analysis\json.txt");
            using (System.IO.FileStream fs = new System.IO.FileStream(System.Environment.CurrentDirectory + @"\analysis\json.zip", System.IO.FileMode.Create))
            {
                var rs = ms.GetBuffer();
                fs.Write(rs, 0, rs.Length);
            }
            return ms;
        }
    }
}
