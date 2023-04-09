using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{

    class Server_Connections:Client_Application_Variables
    {
        protected byte[] error_message = Encoding.UTF8.GetBytes(INotification_Messages.connection_failed_message);
        private byte[] server_response = new byte[Encoding.UTF8.GetBytes("OK").Length];
        private byte[] client_response = Encoding.UTF8.GetBytes("OK");



        private static bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }
            else if(sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                return true;
            }


            return false;
        }




        internal async Task<byte[]> Secure_Server_Connections(string function, string email___or___log_in_session_key___or___account_validation_key, byte[] password___or___binary_file)
        {
            byte[] payload = await payload_serialization.Serialize_Payload(function, email___or___log_in_session_key___or___account_validation_key, password___or___binary_file);

            byte[] server_payload = error_message;




            System.Net.Sockets.Socket client = new System.Net.Sockets.Socket( System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

            try
            {
                client.SendBufferSize = 18000;
                client.ReceiveBufferSize = 18000;
                client.SendTimeout = 1000;
                client.ReceiveTimeout = 1000;




                await client.ConnectAsync(ip_address, port_number);

                System.Net.Sockets.NetworkStream client_network_stream = new System.Net.Sockets.NetworkStream(client);

                try
                {
                    System.Net.Security.SslStream client_secure_socket_layer_stream = new System.Net.Security.SslStream(client_network_stream, false, new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate), null, System.Net.Security.EncryptionPolicy.RequireEncryption);
                    

                    try
                    {
                        client_secure_socket_layer_stream.AuthenticateAsClient(String.Empty, null, ssl_protocol, true);

                        int bytes_per_second = await Connection_Speed_Calculator(ip_address, client_secure_socket_layer_stream);


                        if(bytes_per_second > 0)
                        {
                            byte[] client_payload_size_buffer = BitConverter.GetBytes(payload.Length);

                            await Calculate_Timeout(client, client_payload_size_buffer.Length, bytes_per_second);

                            await client_secure_socket_layer_stream.WriteAsync(client_payload_size_buffer, 0, client_payload_size_buffer.Length);

                            await client_secure_socket_layer_stream.FlushAsync();





                            await Calculate_Timeout(client, server_response.Length, bytes_per_second);

                            await client_secure_socket_layer_stream.ReadAsync(server_response, 0, server_response.Length);

                            await client_secure_socket_layer_stream.FlushAsync();


                            



                            await Calculate_Timeout(client, payload.Length, bytes_per_second);

                            await client_secure_socket_layer_stream.WriteAsync(payload, 0, payload.Length);

                            await client_secure_socket_layer_stream.FlushAsync();






                            byte[] server_payload_size_buffer = new byte[1024];

                            await Calculate_Timeout(client, server_payload_size_buffer.Length, bytes_per_second);

                            await client_secure_socket_layer_stream.ReadAsync(server_payload_size_buffer, 0, server_payload_size_buffer.Length);

                            await client_secure_socket_layer_stream.FlushAsync();






                            await Calculate_Timeout(client, client_response.Length, bytes_per_second);

                            await client_secure_socket_layer_stream.WriteAsync(client_response, 0, client_response.Length);

                            await client_secure_socket_layer_stream.FlushAsync();





                            server_payload = new byte[BitConverter.ToInt32(server_payload_size_buffer)];

                            int total_bytes_read = 0;

                            while (total_bytes_read < server_payload.Length)
                            {
                                total_bytes_read += await client_secure_socket_layer_stream.ReadAsync(server_payload, total_bytes_read, server_payload.Length - total_bytes_read);
                            }


                            server_payload = (await payload_serialization.Deserialize_Payload(server_payload)).Server_Payload;
                            
                        }

                    }
                    catch (Exception e)
                    {
                        server_payload = error_message;
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
                    server_payload = error_message;

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
            catch (Exception E)
            {
                server_payload = error_message;

                if (client != null)
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




        private async Task<int> Connection_Speed_Calculator(System.Net.IPAddress IP_Address, System.Net.Security.SslStream client_secure_socket_layer_stream)
        {
            int round_trip_time_counter = 0;
            int calculated_average_round_trip_time = 0;
            int bytes_per_second = 0;

            byte[] packet = new byte[1500];

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();


            if (IP_Address != null)
            {

            Ping_Test:

                try
                {
                    stopwatch.Start();

                    await client_secure_socket_layer_stream.WriteAsync(packet, 0, packet.Length);

                    await client_secure_socket_layer_stream.ReadAsync(packet, 0, packet.Length);

                    stopwatch.Stop();



                    if (round_trip_time_counter < 10)
                    {
                        calculated_average_round_trip_time += (int)stopwatch.ElapsedMilliseconds;
                        stopwatch.Reset();
                        round_trip_time_counter++;

                        goto Ping_Test;
                    }



                    if (calculated_average_round_trip_time > 0)
                    {
                        calculated_average_round_trip_time = calculated_average_round_trip_time / 10;

                        if (calculated_average_round_trip_time == 0)
                        {
                            calculated_average_round_trip_time = 1;
                        }
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
                catch
                {
                    bytes_per_second = -1;
                }
            }

         

            return bytes_per_second;
        }


        private Task<bool> Calculate_Timeout(System.Net.Sockets.Socket client, int payload_size, int bytes_per_second)
        {
            client.SendBufferSize = payload_size / bytes_per_second + 1000;
            client.ReceiveBufferSize = payload_size / bytes_per_second + 1000;

            return Task.FromResult(true);
        }
    }
}
