using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CGT4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var content = new T5.TextTemplating.TemplatingEngine().ProcessTemplate("",
                 new ScanferTextTemplatingEngineHost()
                 );
        }
    }

    public class ScanferTextTemplatingEngineHost : ITextTemplatingEngineHost
    {
        // Fields
        internal string _templateFileValue;
        private string _namespace = "Scanfer";
        private string _fileExtensionValue = ".cs";
        private Encoding _fileEncodingValue = Encoding.UTF8;
        private CompilerErrorCollection _ErrorCollection;

        /// <summary>
        /// 获取程序集引用的列表。
        /// </summary>
        public IList<string> StandardAssemblyReferences => new string[] {
             typeof(Uri).Assembly.Location,
             typeof(T5.TextTemplating.TemplatingEngine).Assembly.Location
        };

        /// <summary>
        /// 获取命名空间的列表。
        /// </summary>
        public IList<string> StandardImports => new string[] {
            "System", "System.Text", "System.Collections.Generic",
        };
        /// <summary>
        /// 获取所处理文本模板的路径和文件名。
        /// </summary>
        public string TemplateFile
        {
            get =>
                   this._templateFileValue;
            set =>
                this._templateFileValue = value;
        }
        /// <summary>
        /// 由引擎调用以要求指定选项的值。如果您未找到该值，则返回 null。
        /// </summary>
        /// <param name="optionName"></param>
        /// <returns></returns>
        public object GetHostOption(string optionName)
        {
            object returnObject;
            switch (optionName)
            {
                case "CacheAssemblies":
                    returnObject = true;
                    break;
                default:
                    returnObject = null;
                    break;
            }
            return returnObject;
        }
        /// <summary>
        /// 获取文本，它对应于包含部分文本模板文件的请求。
        /// </summary>
        /// <param name="requestFileName"></param>
        /// <param name="content"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 接收来自转换引擎的错误和警告集合。
        /// </summary>
        /// <param name="errors"></param>
        public void LogErrors(CompilerErrorCollection errors)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 提供运行所生成转换类的应用程序域。
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            return AppDomain.CreateDomain("Generation App Domain");
        }
        /// <summary>
        /// 允许主机提供有关程序集位置的附加信息。
        /// </summary>
        /// <param name="assemblyReference"></param>
        /// <returns></returns>
        public string ResolveAssemblyReference(string assemblyReference)
        {
            if (File.Exists(assemblyReference))
            {
                return assemblyReference;
            }
            string path = Path.Combine(Path.GetDirectoryName(this.TemplateFile), assemblyReference);
            if (File.Exists(path))
            {
                return path;
            }
            return "";
        }
        /// <summary>
        /// 在已知指令处理器的友好名称时，返回其类型。
        /// </summary>
        /// <param name="processorName"></param>
        /// <returns></returns>
        public Type ResolveDirectiveProcessor(string processorName)
        {
            string.Compare(processorName, "XYZ", StringComparison.OrdinalIgnoreCase);
            throw new Exception("没有找到指令处理器");
        }
        /// <summary>
        /// 如果在模板文本中未指定某个参数，则为指令处理器解析该参数的值。
        /// </summary>
        /// <param name="directiveId"></param>
        /// <param name="processorName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            if (directiveId == null)
            {
                throw new ArgumentNullException("the directiveId cannot be null");
            }
            if (processorName == null)
            {
                throw new ArgumentNullException("the processorName cannot be null");
            }
            if (parameterName == null)
            {
                throw new ArgumentNullException("the parameterName cannot be null");
            }
            return string.Empty;
        }
        /// <summary>
        /// 允许主机提供完整路径以及给定文件名或相对路径。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ResolvePath(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("the file name cannot be null");
            }
            if (!File.Exists(fileName))
            {
                string path = Path.Combine(Path.GetDirectoryName(this.TemplateFile), fileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return fileName;
        }
        /// <summary>
        /// 通知主机所生成文本输出需要的文件扩展名。
        /// </summary>
        /// <param name="extension"></param>
        public void SetFileExtension(string extension)
        {
            this._fileExtensionValue = extension;
        }
        /// <summary>
        /// 通知主机所生成文本输出需要的编码。
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="fromOutputDirective"></param>
        public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            this._fileEncodingValue = encoding;
        }
    }
}
