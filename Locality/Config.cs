using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Locality
{
    [DataContract]
    public class Space : PropertyChangedBase
    {
        [DataMember]
        public string Id { get; set; }

        private string name;

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; NotifyOfPropertyChange(() => Name); }
        }

        [DataMember]
        public ParametersDictionary Parameters { get; set; }

        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; NotifyOfPropertyChange(() => IsActive); }
        }

        public Space()
        {
            Parameters = new ParametersDictionary();
        }

        [CollectionDataContract(Name = "dict", ItemName = "key", KeyName = "key", ValueName = "value")]
        public class ParametersDictionary : ObservableDictionary<string, object> { }
    }

    [DataContract]
    public class Config
    {
        [DataMember]
        public ObservableCollection<Space> Spaces { get; set; }

        [DataMember]
        public string LastActiveSpaceId { get; set; }

        static string FILE_NAME = ".locality.conf";

        public Config()
        {
            Spaces = new ObservableCollection<Space>();
        }

        public static string GetPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + FILE_NAME;
        }

        public static Config Load()
        {
            if (File.Exists(GetPath()))
            {
                FileStream fs = File.OpenRead(GetPath());
                var result = (Config)new DataContractJsonSerializer(typeof(Config)).ReadObject(fs);
                fs.Close();
                return result;
            }
            else
            {
                return new Config();
            }
        }

        public void Save()
        {
            if (File.Exists(GetPath()))
                File.Delete(GetPath());
            FileStream fs = File.OpenWrite(GetPath());
            new DataContractJsonSerializer(typeof(Config)).WriteObject(fs, this);
            fs.Close();
        }
    }

}
