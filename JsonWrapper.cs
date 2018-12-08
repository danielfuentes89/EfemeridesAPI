using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efemerides
{
    // JsonWrapper to made easy work with DataContract and json.
    public class JsonWrapper<T>
    {

        protected string jsonContent;
        protected String FilePath;
        public T JsonObject;

        public JsonWrapper(string jsonContent)
        {
            this.jsonContent = jsonContent;
        }

        public JsonWrapper(T jsonObject)
        {
            this.JsonObject = jsonObject;
            Sync();
        }

        public JsonWrapper(T jsonObject, string filePath) : this(jsonObject)
        {
            this.FilePath = filePath;
        }

        public static JsonWrapper<T> FromFile(string filePath)
        {
            if (File.Exists(filePath))
                return new JsonWrapper<T>(UTF8Encoding.UTF32.GetString(File.ReadAllBytes(filePath))) { FilePath = filePath };
            else
                return new JsonWrapper<T>(String.Empty) { FilePath = filePath };
        }

        public virtual T Read()
        {
            if (!String.IsNullOrEmpty(this.jsonContent))
            {
                JsonObject = (T)new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(this.jsonContent, typeof(T));
                this.jsonContent = String.Empty;
            }
            else
            {
                JsonObject = (T)Activator.CreateInstance(typeof(T), new object[] { });
            }
            return JsonObject;
        }

        public virtual void Sync()
        {
            this.jsonContent = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(this.JsonObject);
        }

        public virtual string Save()
        {
            if (String.IsNullOrEmpty(this.jsonContent))
                Sync(); // On save If jsonContent is empty, will resync

            if (String.IsNullOrEmpty(this.FilePath))
            {
                this.FilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".json"; ;
            }
            File.WriteAllBytes(this.FilePath, UTF8Encoding.UTF32.GetBytes(this.jsonContent));
            return this.FilePath;
        }

        public void setPath(String filePath)
        {
            this.FilePath = filePath;
        }

        public override string ToString()
        {
            return this.jsonContent;
        }
    }
}
