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
        private int current_connection_speed;
        private byte[] server_response = new byte[Encoding.UTF8.GetBytes("OK").Length];
        private byte[] client_response = Encoding.UTF8.GetBytes("OK");



        private static bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            return false;
        }



        internal async Task<byte[]> Secure_Server_Connections(byte[] payload)
        {
            byte[] server_payload = new byte[1024];


            System.Net.Sockets.Socket client = new System.Net.Sockets.Socket( System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            client.SendBufferSize = 18000;
            client.ReceiveBufferSize = 18000;

            try
            {
                await Connection_Speed_Calculator(ip_address);

                if (current_connection_speed > 0)
                {
                    await client.ConnectAsync(ip_address, port_number);

                    System.Net.Sockets.NetworkStream client_network_stream = new System.Net.Sockets.NetworkStream(client);

                    try
                    {
                        System.Net.Security.SslStream client_secure_socket_layer_stream = new System.Net.Security.SslStream(client_network_stream, false, new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate), null, System.Net.Security.EncryptionPolicy.RequireEncryption);

                        try
                        {
                            client_secure_socket_layer_stream.AuthenticateAsClient("Omega_Drive_Certificate", null, ssl_protocol, true);



                            
                            byte[] client_payload_size_buffer = BitConverter.GetBytes(payload.Length);

                            await Calculate_Timeout(client, client_payload_size_buffer.Length);

                            await client_secure_socket_layer_stream.WriteAsync(client_payload_size_buffer, 0, client_payload_size_buffer.Length);

                            await client_secure_socket_layer_stream.FlushAsync();





                            await Calculate_Timeout(client, server_response.Length);

                            await client_secure_socket_layer_stream.ReadAsync(server_response, 0, server_response.Length);

                            await client_secure_socket_layer_stream.FlushAsync();





                            await Calculate_Timeout(client, payload.Length);

                            await client_secure_socket_layer_stream.WriteAsync(payload, 0, payload.Length);

                            await client_secure_socket_layer_stream.FlushAsync();





                            byte[] server_payload_size_buffer = new byte[1024];

                            await Calculate_Timeout(client, server_payload_size_buffer.Length);

                            await client_secure_socket_layer_stream.ReadAsync(server_payload_size_buffer, 0, server_payload_size_buffer.Length);

                            await client_secure_socket_layer_stream.FlushAsync();





                            await Calculate_Timeout(client, client_response.Length);

                            await client_secure_socket_layer_stream.WriteAsync(client_response, 0, client_response.Length);

                            await client_secure_socket_layer_stream.FlushAsync();





                            server_payload = new byte[BitConverter.ToInt32(server_payload_size_buffer)];

                            int total_bytes_read = 0;

                            while (total_bytes_read < server_payload.Length)
                            {
                                total_bytes_read += await client_secure_socket_layer_stream.ReadAsync(server_payload, total_bytes_read, server_payload.Length - total_bytes_read);
                            }

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

            return server_payload;
        }




        private Task<bool> Connection_Speed_Calculator(System.Net.IPAddress IP_Address)
        {
            int round_trip_time_counter = 0;
            int calculated_average_round_trip_time = 0;
            int bytes_per_second = 0;


            if (IP_Address != null)
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

            current_connection_speed = bytes_per_second;

        Ping_Failed:
            return Task.FromResult(true);
        }


        private Task<bool> Calculate_Timeout(System.Net.Sockets.Socket client, int payload_size)
        {
            client.SendBufferSize = payload_size / current_connection_speed + 1000;
            client.ReceiveBufferSize = payload_size / current_connection_speed + 1000;

            return Task.FromResult(true);
        }
    }
}
