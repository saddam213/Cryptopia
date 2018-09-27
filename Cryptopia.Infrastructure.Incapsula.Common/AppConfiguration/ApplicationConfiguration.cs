using System;
using System.Configuration;

using Cryptopia.Infrastructure.Incapsula.Common.Constants;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using System.Reflection;
using System.Runtime.Serialization;

namespace Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration
{
    [DataContract]
    public class ApplicationConfiguration : IApplicationConfiguration
    {
        [IgnoreDataMember]
        private Configuration _config;

        public ApplicationConfiguration()
        {
            string configPath = Assembly.GetEntryAssembly().Location;

            try
            {
                _config = ConfigurationManager.OpenExeConfiguration(configPath);
            }
            catch (Exception)
            { }
        }

        [DataMember]
        public TargetSite TargetWebsite
        {
            get
            {
                if (_config != null)
                {
                    string target = GetAppSetting<string>(StringConstants.appSetting_TargetWebsite);
                    return GetTargetSite(target);
                }

                return TargetSite.None;
            }
        }

        [DataMember]
        public string IncapsulaApiId
        {
            get
            {
                if (_config != null)
                {
                    return GetAppSetting<string>(StringConstants.appSetting_ApiId);
                }

                return string.Empty;
            }
        }

        [DataMember]
        public string IncapsulaApiKey
        {
            get
            {
                if (_config != null)
                {
                    return GetAppSetting<string>(StringConstants.appSetting_ApiKey);
                }

                return string.Empty;
            }
        }

        [DataMember]
        public string IncapsulaAccountId
        {
            get
            {
                if (_config != null)
                {
                    return GetAppSetting<string>(StringConstants.appSetting_ApiAccount);
                }

                return string.Empty;
            }
        }

        [DataMember]
        public string IncapsulaEndpoint
        {
            get
            {
                if (_config != null)
                {
                    return GetAppSetting<string>(StringConstants.appSetting_ApiEndpoint);
                }

                return string.Empty;
            }
        }

        [DataMember]
        public int IncapsulaApiPollInterval
        {
            get
            {
                if (_config != null)
                {
                    return GetAppSetting<int>(StringConstants.appSetting_ApiPollingInterval) * 1000;
                }

                return -1;
            }
        }

        #region Private Methods

        private TargetSite GetTargetSite(string siteName)
        {
            switch (siteName)
            {
                case StringConstants.appSetting_DevDotCoDotNZ:
                case StringConstants.appSetting_DevDotCom:
                    return TargetSite.Cryptopia_Development;
                case StringConstants.appSetting_ProductionDotCoDotNZ:
                case StringConstants.appSetting_ProductionDotCom:
                    return TargetSite.Cryptopia_Production;
                default:
                    return TargetSite.None;
            }
        }

        private T GetAppSetting<T>(string key)
        {
            T returnValue = default(T);

            if (_config != null)
            {
                KeyValueConfigurationElement element = _config.AppSettings.Settings[key];
                Type returnType = typeof(T);

                if (returnType == typeof(string))
                {
                    if (element != null)
                    {
                        string value = element.Value;
                        if (!string.IsNullOrEmpty(value))
                            returnValue = (T)Convert.ChangeType(value, typeof(T));
                    }
                    else
                        returnValue = (T)Convert.ChangeType(string.Empty, typeof(T));
                }

                if (returnType == typeof(int))
                {
                    if (element != null)
                    {
                        int result;
                        if (int.TryParse(element.Value, out result))
                            returnValue = (T)Convert.ChangeType(result, typeof(T));
                    }
                    else
                        returnValue = (T)Convert.ChangeType(-1, typeof(T));
                }
            }

            return returnValue;
        }

        #endregion
    }
}
