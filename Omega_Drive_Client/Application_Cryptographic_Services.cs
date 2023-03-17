using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    class Application_Cryptographic_Services
    {
        protected static Task<bool> Load_Certificate_Authority(string certificate_path, string password)
        {
            try
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 server_certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate_path, password);

                try
                {



                    System.Security.Cryptography.X509Certificates.X509Store certificate_store = new System.Security.Cryptography.X509Certificates.X509Store(System.Security.Cryptography.X509Certificates.StoreName.Root, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);

                    try
                    {
                        certificate_store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadWrite);
                        certificate_store.Add(server_certificate);
                    }
                    catch (Exception E)
                    {

                    }
                    finally
                    {
                        if (certificate_store != null)
                        {
                            certificate_store.Dispose();
                        }
                    }
                }
                catch (Exception E)
                {

                }
                finally
                {
                    if (server_certificate != null)
                    {
                        server_certificate.Dispose();
                    }
                }
            }
            catch (Exception E)
            {

            }

            return Task.FromResult(true);
        }
    }
}
