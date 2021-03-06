﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PdfTextReader;

namespace ParserFrontend.Logic
{
    public class OutputFiles
    {
        private readonly IVirtualFS _webFs;

        public OutputFiles(IVirtualFS virtualFS)
        {
            this._webFs = virtualFS;
        }

        public Dictionary<string, string> Load(string basename)
        {
            using (var stream = _webFs.OpenReader($"output/{basename}/filelist.json"))
            using (var file = new StreamReader(stream))
            {
                string content = file.ReadToEnd();
                var fileListInfo = JsonConvert.DeserializeObject<Dictionary<string,string>>(content);

                return fileListInfo;
            }
        }

        public void Save(string basename, Dictionary<string,string> fileListInfo)
        {
            using (var stream = _webFs.OpenWriter($"output/{basename}/filelist.json"))
            using (var file = new StreamWriter(stream))
            {
                string content = JsonConvert.SerializeObject(fileListInfo, Formatting.Indented);
                file.Write(content);
            }
        }

        string GetFilename(string basename, string file)
        {
            var fileList = Load(basename);

            if (!fileList.ContainsKey(file))
                throw new FileNotFoundException($"basename={basename}; file={file}");

            return fileList[file];
        }

        Stream OpenReader(string basename, string file)
        {
            string filename = GetFilename(basename, file);
            
            return _webFs.OpenReader(filename);
        }

        Stream OpenReader(string basename, string file, string arg1)
        {
            string filename_format = GetFilename(basename, file);
            string filename = String.Format(filename_format, arg1);

            return _webFs.OpenReader(filename);
        }

        public Stream GetOutputFile(string basename)
        {
            return OpenReader(basename, "stage3");
        }

        public string GetOutputTree(string basename)
        {
            using (var stream = OpenReader(basename, "tree"))
            using (var file = new StreamReader(stream))
            {
                string content = file.ReadToEnd();
                return content;
            }
        }

        public string GetOutputArtigo(string basename, int idArtigo)
        {
            using (var stream = OpenReader(basename, "artigosGN", idArtigo.ToString()))
            using (var file = new StreamReader(stream))
            {
                string content = file.ReadToEnd();
                return content;
            }
        }
    }
}
