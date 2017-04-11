using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Song_Public
{
    /// <summary>
    /// 实现数据库及源码备份 调用实例：
    /// </summary>
    class BackupsHelper
    {
        /// <summary>
        /// 备份数据库 可能需要等待几秒钟 建议使用异步
        /// 调用实例 BackupsHelper.database()
        /// </summary>
        /// <returns></returns>
        public static string database() {

            string msg = "";//消息
            string fileName = "MiddleSchool_" + DateTime.Now.ToString(("yyyyMMddHHmmss")) + ".bak";//备份地址
            try
            {
                SqlConnection connection = new SqlConnection("Data Source=" + "." + ";initial catalog=" + "MiddleSchool" + ";user id=" + "sa" + ";password=" + "1qaz@WSX" + ";");
                SqlCommand command = new SqlCommand("use MiddleSchool;backup database MiddleSchool to disk=\'d:\\database\\" + fileName + "\';", connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                msg = "备份成功！";
            }
            catch (Exception ee)
            {
                msg = "数据库备份失败，错误原因：" + ee.ToString();
            }
            return msg;
        }


        /// <summary>
        /// 备份文件 使用压缩方式  可能需要等待几分钟 建议使用异步
        /// 调用方式 BackupsHelper.zip()
        /// </summary>
        /// <param name="strFile">项目地址  绝对路径"D:\\webroot\\</param>
        /// <param name="savapath">保存地址 绝对路径"D:\\webroot\\</param>
        /// <returns></returns>
        public static string zip(string strFile,string savapath) {
            // string strFile = "D:\\webroot\\...";
            //string strZip = "d:\\codeback\\MiddleSchool" + DateTime.Now.ToString(("yyyyMMddHHmmss")) + ".zip";
            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar)
                strFile += Path.DirectorySeparatorChar;
            ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(savapath));
            s.SetLevel(9); //0-9压缩质量 
            zip(strFile, s, strFile);
            s.Finish();
            s.Close();
            return "压缩成功";
        }

        private  static void zip(string strFile, ZipOutputStream s, string staticFile)
        {
            Crc32 crc = new Crc32();

            if (strFile[strFile.Length - 1] != Path.DirectorySeparatorChar) strFile += Path.DirectorySeparatorChar;
            string[] filenames = Directory.GetFileSystemEntries(strFile);
            foreach (string file in filenames)
            {
                if (Directory.Exists(file))
                {
                    zip(file, s, staticFile);
                }
                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    FileStream fs = System.IO.File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string tempfile = file.Replace(staticFile + "\\", "");

                    ZipEntry entry = new ZipEntry(tempfile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }

        }
    }
}
