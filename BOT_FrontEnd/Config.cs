using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace BOT_FrontEnd
{
    public class Config : IXmlSerializable
    {
        private XmlDocument configXML;

        public ChannelConfigValues[] channel_config { get; private set; }
        public bool z_persist { get; private set; }
        public bool z_accumulating { get; private set; }

        public ControllerProperty[] inputMapping { get; private set; }
        public bool[] inputInverted { get; private set; }

        public String[] precisionFormat { get; private set; }
        public String[] channelPrefix { get; private set; }
        public double[] channelGain { get; private set; }

        public String stopCommand { get; private set; }
        public String chanSeparator { get; private set; }
        public String cmdPrefixAbsolute { get; private set; }
        public String cmdPrefixRelative { get; private set; }

        public Config()
        {
            InitArrays();
            configXML = new XmlDocument();

            cmdPrefixAbsolute = "G90";
            cmdPrefixRelative = "G91";
        }

        public Config(String xmlConfigFileName)
        {
            InitArrays();
            configXML = new XmlDocument();
            configXML.Load(xmlConfigFileName);
        }

        private void InitArrays()
        {
            channel_config = new ChannelConfigValues[(int)ChannelNumber.NUM_CHANNELS];
            inputMapping = new ControllerProperty[(int)ChannelNumber.NUM_CHANNELS];
            precisionFormat = Enumerable.Repeat<String>("", (int)ChannelNumber.NUM_CHANNELS).ToArray();
            channelPrefix = Enumerable.Repeat<String>("", (int)ChannelNumber.NUM_CHANNELS).ToArray();
            channelGain = Enumerable.Repeat<double>(0.0, (int)ChannelNumber.NUM_CHANNELS).ToArray();
            inputInverted = Enumerable.Repeat<bool>(false, (int)ChannelNumber.NUM_CHANNELS).ToArray();
        }

        public void GetConfig(Guid controller_id)
        {
            XmlNode cNode = configXML.SelectNodes(String.Format("//Controller[@GUID='{0}']", controller_id.ToString())).Item(0);

            if(!cNode.HasChildNodes)
            {
                cNode = configXML.SelectNodes("//Controller[@GUID='Default']").Item(0);
            }

            ParseControllerConfig(cNode);
        }

        public void GetDefaultConfig()
        {
            XmlNode cNode = configXML.SelectNodes("//Controller[@GUID='Default']").Item(0);
            ParseControllerConfig(cNode);
        }

        private void ParseControllerConfig(XmlNode configRoot)
        {
            int temp_mapping = 0;

            for(int i = 0; i < (int)ChannelNumber.NUM_CHANNELS; i++)
            {
                channel_config[i] = new ChannelConfigValues();
                inputMapping[i] = new ControllerProperty();

                temp_mapping = int.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["map"].Value);
                inputMapping[i] = (ControllerProperty)Math.Abs(temp_mapping);
                if(temp_mapping < 0)
                {
                    inputInverted[i] = true;
                }

                channel_config[i].MIN = double.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["min"].Value);
                channel_config[i].MAX = double.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["max"].Value);
                channel_config[i].CENTER = double.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["default"].Value);

                precisionFormat[i] = configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["precision"].Value;
                channelGain[i] = double.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["gain"].Value);
                channelPrefix[i] = configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).InnerXml;
            }

            z_persist = Boolean.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue(ChannelNumber.CH1))).Item(0).Attributes["persist"].Value);
            z_accumulating = Boolean.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue(ChannelNumber.CH1))).Item(0).Attributes["accum"].Value);

            chanSeparator = configRoot.SelectNodes(".//ChannelSeparator").Item(0).InnerXml;
            stopCommand = configRoot.SelectNodes(".//StopCommand").Item(0).InnerXml;
            cmdPrefixAbsolute = configRoot.SelectNodes(".//StartOfFrame[@type='absolute']").Item(0).InnerXml;
            cmdPrefixRelative = configRoot.SelectNodes(".//StartOfFrame[@type='relative']").Item(0).InnerXml;
        }

        private void SetControllerConfig(XmlNode configRoot)
        {
            int temp_mapping = 0;

            for(int i = 0; i < (int)ChannelNumber.NUM_CHANNELS; i++)
            {
                configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["min"].Value = channel_config[i].MIN.ToString();
                configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["max"].Value = channel_config[i].MAX.ToString();
                configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["default"].Value = channel_config[i].CENTER.ToString();

                configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["precision"].Value = precisionFormat[i];
                configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["gain"].Value = channelGain[i].ToString();
                configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).InnerXml = channelPrefix[i];

                temp_mapping = (int)inputMapping[i];
                if(inputInverted[i])
                {
                    temp_mapping *= -1;
                }
                configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["map"].Value = temp_mapping.ToString();
            }

            configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue(ChannelNumber.CH1))).Item(0).Attributes["persist"].Value = z_persist.ToString();
            configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue(ChannelNumber.CH1))).Item(0).Attributes["accum"].Value = z_accumulating.ToString();

            configRoot.SelectNodes(".//ChannelSeparator").Item(0).InnerXml = chanSeparator;
            configRoot.SelectNodes(".//StopCommand").Item(0).InnerXml = stopCommand;
            configRoot.SelectNodes(".//StartOfFrame[@type='absolute']").Item(0).InnerXml = cmdPrefixAbsolute;
            configRoot.SelectNodes(".//StartOfFrame[@type='relative']").Item(0).InnerXml = cmdPrefixRelative;
        }

        public void SaveToFile(String filename, Controller controller)
        {
            if(configXML != null)
            {
                XmlNode cNode = configXML.SelectNodes(String.Format("//Controller[@GUID='{0}']", controller.DeviceGuid.ToString())).Item(0);

                inputMapping = controller.ChannelMapping;
                inputInverted = controller.ChannelInverted;

                SetControllerConfig(cNode);
                configXML.Save(filename);
            }
        }

        public void SaveToFile(String filename)
        {
            if (configXML != null)
            {
                XmlNode cNode = configXML.SelectNodes("//Controller[@GUID='Default']").Item(0);
                SetControllerConfig(cNode);
                configXML.Save(filename);
            }
        }

        public void SetConfigParams(ChannelConfigValues[] pConfig, bool pPersist, bool pAccum)
        {
            channel_config = pConfig;
            z_persist = pPersist;
            z_accumulating = pAccum;
        }

        #region XML Serialization Functions
        public XmlElement ToXmlElement()
        {
            XmlDocument doc = new XmlDocument();

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                new System.Xml.Serialization.XmlSerializer(typeof(Config)).Serialize(writer, this);
            }

            return doc.DocumentElement;
        }

        // Xml Serialization Infrastructure
        public void WriteXml(XmlWriter writer)
        {
            XmlDocument doc = new XmlDocument();

            //write Name field
            //writer.WriteElementString("Name", Name);

            
            writer.WriteEndElement();
        }

        public void ReadXml(XmlReader reader)
        {
            //EMPTY
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }
        #endregion

    }
}
