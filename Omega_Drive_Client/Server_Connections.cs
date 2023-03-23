using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    class Server_Connections:Client_Application_Variables
    {

        private static bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            System.Diagnostics.Debug.WriteLine("Policy: " + sslPolicyErrors.ToString());


            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            return false;
        }

        internal async Task<byte[]> Secure_Server_Connections()
        {
            System.Net.Sockets.Socket client = new System.Net.Sockets.Socket( System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            client.SendBufferSize = 18000;
            client.ReceiveBufferSize = 18000;

            int connection_speed = await Connection_Speed_Calculator(((IPEndPoint)client.RemoteEndPoint).Address);

            try
            {
                if(connection_speed > 0)
                {
                    int timeout = 18000 / connection_speed + 1000;

                    client.SendTimeout = timeout;
                    client.ReceiveTimeout = timeout;

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


        private static Task<int> Connection_Speed_Calculator(System.Net.IPAddress IP_Address)
        {
            int round_trip_time_counter = 0;
            int calculated_average_round_trip_time = 0;
            int bytes_per_second = 0;


            if(IP_Address != null)
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();


                System.Net.NetworkInformation.PingOptions ping_options = new System.Net.NetworkInformation.PingOptions();
                ping_options.DontFragment = true;



                while (round_trip_time_counter < 10)
                {
                    System.Net.NetworkInformation.PingReply ping_reply = ping.Send(IP_Address, 100, new byte[1500], ping_options);

                    if (ping_reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        calculated_average_round_trip_time += (int)ping_reply.RoundtripTime;
                    }
                    else
                    {
                        bytes_per_second = -1;
                        goto Ping_Failed;
                    }

                    round_trip_time_counter++;
                }



                if (calculated_average_round_trip_time > 0)
                {
                    calculated_average_round_trip_time = calculated_average_round_trip_time / 10;
                }
                else
                {
                    calculated_average_round_trip_time = 1;
                }


                bytes_per_second = 24 / calculated_average_round_trip_time * 125000;

                if (bytes_per_second < 1)
                {
                    bytes_per_second = 1;
                }
            }


        Ping_Failed:
            return Task.FromResult(bytes_per_second);

        }
    }
}
