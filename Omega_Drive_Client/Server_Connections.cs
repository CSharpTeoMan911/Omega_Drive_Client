using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    class Server_Connections:Client_Application_Variables
    {

        public static bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            System.Diagnostics.Debug.WriteLine("Policy: " + sslPolicyErrors.ToString());


            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            return false;
        }

        protected static async Task<byte[]> Secure_Server_Connections()
        {
            System.Net.Sockets.Socket client = new System.Net.Sockets.Socket( System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            client.ReceiveTimeout = 1000;
            client.SendTimeout = 1000;
            try
            {
                await client.ConnectAsync(ip_address, port_number);
            
                System.Net.Sockets.NetworkStream client_network_stream = new System.Net.Sockets.NetworkStream(client);

                try
                {
                    System.Net.Security.SslStream client_secure_socket_layer_stream = new System.Net.Security.SslStream(client_network_stream, false, new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate), null);

                    try
                    {
                        client_secure_socket_layer_stream.AuthenticateAsClient("Omega_Drive_Certificate", null, ssl_protocol, true);
                    }
                    catch (Exception E)
                    {
                        System.Diagnostics.Debug.WriteLine(E.Message);
                        if (client_secure_socket_layer_stream != null)
                        {
                            client_secure_socket_layer_stream.Close();
                        }
                    }
                    finally
                    {
                        if (client_secure_socket_layer_stream != null)
                        {
                            await client_secure_socket_layer_stream.DisposeAsync();
                        }
                    }
                }
                catch (Exception E)
                {
                    System.Diagnostics.Debug.WriteLine(E.Message);
                    if (client_network_stream != null)
                    {
                        client_network_stream.Close();
                    }
                }
                finally
                {
                    if (client_network_stream != null)
                    {
                        client_network_stream.Close();
                        await client_network_stream.DisposeAsync();
                    }
                }

            }
            catch(Exception E)
            {
                System.Diagnostics.Debug.WriteLine(E.Message);

                if(client != null)
                {
                    client.Close();
                }
            }
            finally
            {
                if (client != null)
                {
                    client.Close();
                    client.Dispose();
                }
            }

            return new byte[0];
        }
    }
}
