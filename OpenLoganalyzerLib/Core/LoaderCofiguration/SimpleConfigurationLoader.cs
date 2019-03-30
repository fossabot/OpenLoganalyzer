﻿using OpenLoganalyzerLib.Core.Interfaces;
using OpenLoganalyzerLib.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;


namespace OpenLoganalyzerLib.Core.LoaderCofiguration
{
    public class SimpleConfigurationLoader : ILoaderConfigurationLoader
    {
        public event EventHandler LoadingError;

        public ILoaderConfiguration Load(string pathToFile)
        {
            ILoaderConfiguration returnConfiguration = null;

            if (!File.Exists(pathToFile))
            {
                return returnConfiguration;
            }

            JsonSerializer jsonSerializer = new JsonSerializer();
            try
            {
                string content = File.ReadAllText(pathToFile);
                JsonSimpleConfiguration container = JsonConvert.DeserializeObject<JsonSimpleConfiguration>(content);
                returnConfiguration = container.GetLoaderConfiguration();
            }
            catch (Exception ex)
            {
                EventHandler handler = LoadingError;
                if (handler != null)
                {
                    EventArgs loadingError = new ErrorLoadingEvent(ex);
                    handler.Invoke(this, loadingError);
                }
            }
            return returnConfiguration;
        }

        public bool Save(ILoaderConfiguration configuration, string filePath)
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                jsonSerializer.Serialize(writer, configuration.GetSaveableObject());
            }
            return true;
        }
    }
}
