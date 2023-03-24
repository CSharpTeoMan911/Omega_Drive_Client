﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    internal class Payload_Serialization
    {


        public async Task<byte[]> Serialize_Payload<Password___Or___Binary_File>(string function, string email___or___log_in_session_key___or___account_validation_key, Password___Or___Binary_File password___or___binary_file)
        {
            byte[] serialized_payload = new byte[1024];


            System.IO.MemoryStream payload_stream = new System.IO.MemoryStream();

            try
            {
                Client_WSDL_Payload client_WSDL_Payload = new Client_WSDL_Payload();
                client_WSDL_Payload.Function = Convert.ToBase64String(Encoding.UTF8.GetBytes(function));
                client_WSDL_Payload.Email___Or___Log_In_Session_Key___Or___Account_Validation_Key = Convert.ToBase64String(Encoding.UTF8.GetBytes(email___or___log_in_session_key___or___account_validation_key));

                if (password___or___binary_file.GetType() == typeof(byte[]))
                {
                    client_WSDL_Payload.Password___Or___Binary_Content = Convert.ToBase64String(password___or___binary_file as byte[]);
                }
                else
                {
                    client_WSDL_Payload.Password___Or___Binary_Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(password___or___binary_file as string));
                }



                System.Xml.Serialization.XmlSerializer payload_serialiser = new System.Xml.Serialization.XmlSerializer(client_WSDL_Payload.GetType());
                payload_serialiser.Serialize(payload_stream, client_WSDL_Payload);

                serialized_payload = payload_stream.ToArray();
                await payload_stream.FlushAsync();
            }
            catch (Exception E)
            {

                if (payload_stream != null)
                {
                    payload_stream.Close();
                }
            }
            finally
            {
                if (payload_stream != null)
                {
                    payload_stream.Close();
                    payload_stream.Dispose();
                }
            }


            return serialized_payload;
        }





        public Task<Server_WSDL_Payload> Deserialize_Payload(byte[] payload)
        {
            Server_WSDL_Payload server_WSDL_Payload = new Server_WSDL_Payload();


            System.IO.TextReader payload_stream = new System.IO.StringReader(Encoding.UTF8.GetString(payload));

            try
            {
                System.Xml.Serialization.XmlSerializer payload_deserialiser = new System.Xml.Serialization.XmlSerializer(server_WSDL_Payload.GetType());
                server_WSDL_Payload = (Server_WSDL_Payload)payload_deserialiser?.Deserialize(payload_stream);

                server_WSDL_Payload.Server_Payload = Encoding.UTF8.GetString(Convert.FromBase64String(server_WSDL_Payload.Server_Payload));
            }
            catch (Exception E)
            {
                if (payload_stream != null)
                {
                    payload_stream.Close();
                }
            }
            finally
            {
                if (payload_stream != null)
                {
                    payload_stream.Close();
                    payload_stream.Dispose();
                }
            }

            return Task.FromResult(server_WSDL_Payload);
        }
    }
}
