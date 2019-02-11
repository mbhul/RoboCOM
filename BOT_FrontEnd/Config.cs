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

        public String[] precisionFormat { get; private set; }
        public double[] channelGain { get; private set; }

        public String stopCommand { get; private set; }
        public String chanSeparator { get; private set; }

        public Config()
        {
            InitArrays();
            configXML = new XmlDocument();
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
            precisionFormat = Enumerable.Repeat<String>("", (int)ChannelNumber.NUM_CHANNELS).ToArray();
            channelGain = Enumerable.Repeat<double>(0.0, (int)ChannelNumber.NUM_CHANNELS).ToArray();
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
            for(int i = 0; i < (int)ChannelNumber.NUM_CHANNELS; i++)
            {
                channel_config[i] = new ChannelConfigValues();
                channel_config[i].MIN = int.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["min"].Value);
                channel_config[i].MAX = int.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["max"].Value);
                channel_config[i].CENTER = int.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["default"].Value);

                precisionFormat[i] = configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["precision"].Value;
                channelGain[i] = double.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue((ChannelNumber)i))).Item(0).Attributes["gain"].Value);
            }

            z_persist = Boolean.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue(ChannelNumber.CH1))).Item(0).Attributes["persist"].Value);
            z_accumulating = Boolean.Parse(configRoot.SelectNodes(String.Format(".//Channel[@param='{0}']", StringEnum.GetStringValue(ChannelNumber.CH1))).Item(0).Attributes["accum"].Value);

            chanSeparator = configRoot.SelectNodes(".//ChannelSeparator").Item(0).InnerXml;
            stopCommand = configRoot.SelectNodes(".//StopCommand").Item(0).InnerXml;
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
            // personName = reader.ReadString();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }
        #endregion

    }
}
