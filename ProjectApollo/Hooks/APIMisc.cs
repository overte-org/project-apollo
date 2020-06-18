﻿//   Copyright 2020 Vircadia
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Project_Apollo.Configuration;
using Project_Apollo.Entities;
using Project_Apollo.Registry;

using Newtonsoft.Json;

namespace Project_Apollo.Hooks
{
    public class APIMisc
    {
        public static readonly string _logHeader = "[APIMisc]";

        // == GET /api/metaverse_info ==================================
        private static string _metaverse_info;

        [APIPath("/api/metaverse_info", "GET", true)]
        public RESTReplyData get_metaverse_info(RESTRequestData pReq, List<string> pArgs)
        {
            RESTReplyData replyData = new RESTReplyData();  // The HTTP response info

            if (_metaverse_info == null)
            {
                JObject jo = new JObject
                {

                    // Start with the basic parameters
                    ["metaverse_name"] = Context.Params.P<string>(AppParams.P_METAVERSE_NAME),
                    ["metaverse_nick_name"] = Context.Params.P<string>(AppParams.P_METAVERSE_NICKNAME),
                    ["metaverse_url"] = Context.Params.P<string>(AppParams.P_METAVERSE_SERVER_URL),
                    ["ice_server_url"] = Context.Params.P<string>(AppParams.P_DEFAULT_ICE_SERVER),
                    ["metaverse_server_version"] = ThisAssembly.AssemblyInformationalVersion
                };

                // See if there are additions in the info file
                string infoFile = EntityStorage.GenerateAbsStorageLocation(null, Context.Params.P<string>(AppParams.P_METAVERSE_INFO_FILE));
                if (File.Exists(infoFile))
                {
                    try
                    {
                        string fContents = File.ReadAllText(infoFile);
                        JObject jContents = JObject.Parse(fContents);
                        foreach (JProperty jprop in jContents.Properties())
                        {
                            jo[jprop.Name] = jprop.Value;
                        }
                    }
                    catch (Exception e)
                    {
                        Context.Log.Error("{0} Exception reading metaverse info file {1}: {2}",
                                            _logHeader, infoFile, e);
                    }
                }   
                _metaverse_info = JsonConvert.SerializeObject(jo, Formatting.Indented);
            }

            replyData.Body = _metaverse_info;

            return replyData;
        }
    }
}
