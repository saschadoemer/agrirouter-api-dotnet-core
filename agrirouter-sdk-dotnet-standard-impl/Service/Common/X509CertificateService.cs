using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Agrirouter.Api.Dto.Onboard;
using Agrirouter.Api.Dto.Onboard.Inner;
using Agrirouter.Api.Exception;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Agrirouter.Impl.Service.Common
{
    /// <summary>
    /// Service to create X509 certificates for the communication with the AR.
    /// </summary>
    public class X509CertificateService
    {
        private static X509Certificate GetCertificate(Authentication authentication)
        {
            switch (authentication.Type)
            {
                case "P12":
                    return new X509Certificate2(
                        Convert.FromBase64String(authentication.Certificate),
                        authentication.Secret,
                        X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                case "PEM":
                {
                    var pemReader = new PemReader(
                        new StringReader(authentication.Certificate),
                        new PasswordFinder(authentication.Secret));

                    RSA privateKey;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                        RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        privateKey = GivenToRsa((RsaPrivateCrtKeyParameters) pemReader.ReadObject());
                    }
                    else
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            privateKey = DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters) pemReader.ReadObject());
                        }
                        else
                        {
                            throw new CouldNotCreateCertificateForOsException(
                                $"Could not create a certificate for '${RuntimeInformation.OSDescription}'");
                        }
                    }

                    var certificate = pemReader.ReadPemObject();
                    if (certificate.Type == "CERTIFICATE")
                    {
                        return new X509Certificate2(certificate.Content, (String) null,
                                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable)
                            .CopyWithPrivateKey(privateKey);
                    }

                    break;
                }
            }

            throw new CouldNotCreateCertificateForTypeException(
                $"Could not create a certificate for the type '${authentication.Type}'");

        }

        /// <summary>
        /// Create a certificate from the given router device from the AR.
        /// </summary>
        /// <param name="routerDevice">-</param>
        /// <returns>An X509 certificate to use for the communication between endpoint and AR.</returns>
        /// <exception cref="CouldNotCreateCertificateForTypeException">-</exception>

        public static X509Certificate GetCertificate(RouterDevice routerDevice)
        {
            return GetCertificate(routerDevice.Authentication);
        }

        /// <summary>
        /// Create a certificate from the given onboarding response from the AR.
        /// </summary>
        /// <param name="onboardResponse">-</param>
        /// <returns>An X509 certificate to use for the communication between endpoint and AR.</returns>
        /// <exception cref="CouldNotCreateCertificateForTypeException">-</exception>
        public static X509Certificate GetCertificate(OnboardResponse onboardResponse)
        {
            return GetCertificate(onboardResponse.Authentication);
        }
        
        /// <summary>
        /// Internal implementation for Linux and OSX to support PEM certificate creation.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static RSA GivenToRsa(RsaPrivateCrtKeyParameters parameters)
        {
            var rp = DotNetUtilities.ToRSAParameters(parameters);
            return RSA.Create(rp);
        }

        /// <summary>
        /// Internal implementation of the password finder to pass the secret to the PEM reader.
        /// </summary>
        private class PasswordFinder : IPasswordFinder
        {
            private readonly string _password;

            public PasswordFinder(string password)
            {
                this._password = password;
            }

            public char[] GetPassword()
            {
                return _password.ToCharArray();
            }
        }
    }
}
