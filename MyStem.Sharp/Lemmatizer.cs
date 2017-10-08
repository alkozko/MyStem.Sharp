using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace MyStem.Sharp
{
    public class Lemmatizer
    {
        private readonly Regex _cleanRegex;
        private static readonly object LockObject = new object();
        private readonly Process _myStemProcess;
        private readonly StreamReader _streamReader;

        public Lemmatizer(string path)
        {
            _cleanRegex = new Regex(@"[^\w\d\p{P}]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _myStemProcess = CreateProcess(path);
            _streamReader = new StreamReader(_myStemProcess.StandardOutput.BaseStream, Encoding.UTF8);
        }

        public WordDefenition[] Lemmatize(string text)
        {
            var cleanText = _cleanRegex.Replace(text ?? ""," ");

            string result;
            lock (LockObject)
            {
                result = GetProcessOutput(_myStemProcess, _streamReader, cleanText);
            }
            return JArray.Parse(result)
                .ToObject<List<WordDefenition>>()
                .Select(def => new WordDefenition(def.Text.Replace("\n", "").Replace(@"\s", ""), def.Analysis))
                .ToArray();
        }

        private string GetProcessOutput(Process myStemProcess, StreamReader streamReader, string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text + "\r\n");
            myStemProcess.StandardInput.BaseStream.Write(buffer, 0, buffer.Length);
            myStemProcess.StandardInput.BaseStream.Flush();
            return streamReader.ReadLine();
        }

        private ProcessStartInfo CreateStartInfo(string path)
        {
            return new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = path,
                Arguments = "-cdis --format json"
            };
        }

        private Process CreateProcess(string path)
        {
            var process = new Process {StartInfo = CreateStartInfo(path)};
            process.Start();
            return process;
        }
    }
}
