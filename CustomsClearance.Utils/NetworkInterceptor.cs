using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace CustomsClearance.Utils
{
    public class NetworkInterceptor
    {
        private static NetworkInterceptor _instance = null;
        private static ExplicitProxyEndPoint _explicitEndPoint;
        private NetworkInterceptor()
        {
        }

        public static NetworkInterceptor Instance => _instance ?? (_instance = new NetworkInterceptor());
        private ProxyServer _proxyServer;
        public void Destroy()
        {
            if (_proxyServer != null)
            {
                //Unsubscribe & Quit
                _explicitEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
                _proxyServer.BeforeRequest -= OnRequest;
                _proxyServer.BeforeResponse -= OnResponse;
                _proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
                _proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;

                _proxyServer.Stop();
                _proxyServer.Dispose();
            }

        }

        public void Start()
        {
            if (_proxyServer !=null)
            {
                _proxyServer.Start();
            }
        }

        public void Stop()
        {
        }
        public void Initialize()
        {
            _proxyServer = new ProxyServer();

            //locally trust root certificate used by this proxy 
            _proxyServer.CertificateManager.TrustRootCertificate(true);
            //proxyServer.CertificateManager.TrustRootCertificate = true;

            //optionally set the Certificate Engine
            //Under Mono only BouncyCastle will be supported
            //proxyServer.CertificateManager.CertificateEngine = Network.CertificateEngine.BouncyCastle;

            _proxyServer.BeforeRequest += OnRequest;
            _proxyServer.BeforeResponse += OnResponse;
            _proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            _proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;


            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true)
            {
                //Use self-issued generic certificate on all https requests
                //Optimizes performance by not creating a certificate for each https-enabled domain
                //Useful when certificate trust is not required by proxy clients
                //GenericCertificate = new X509Certificate2(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "genericcert.pfx"), "password")
            };

            //Fired when a CONNECT request is received
            explicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;

            //An explicit endpoint is where the client knows about the existence of a proxy
            //So client sends request in a proxy friendly manner
            _proxyServer.AddEndPoint(explicitEndPoint);
            _proxyServer.Start();

            //Transparent endpoint is useful for reverse proxy (client is not aware of the existence of proxy)
            //A transparent endpoint usually requires a network router port forwarding HTTP(S) packets or DNS
            //to send data to this endPoint
            var transparentEndPoint = new TransparentProxyEndPoint(IPAddress.Any, 8001, true)
            {
                //Generic Certificate hostname to use
                //when SNI is disabled by client
                GenericCertificateName = "google.com"
            };

            _proxyServer.AddEndPoint(transparentEndPoint);

            //proxyServer.UpStreamHttpProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };
            //proxyServer.UpStreamHttpsProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };

            foreach (var endPoint in _proxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

            //Only explicit proxies can be set as system proxy!
            _proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            _proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);
        }

        #region source

        public void Run()
        {
            _proxyServer = new ProxyServer();

            //locally trust root certificate used by this proxy 
            _proxyServer.CertificateManager.TrustRootCertificate(true);
            //proxyServer.CertificateManager.TrustRootCertificate = true;

            //optionally set the Certificate Engine
            //Under Mono only BouncyCastle will be supported
            //proxyServer.CertificateManager.CertificateEngine = Network.CertificateEngine.BouncyCastle;

            _proxyServer.BeforeRequest += OnRequest;
            _proxyServer.BeforeResponse += OnResponse;
            _proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            _proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;


            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true)
            {
                //Use self-issued generic certificate on all https requests
                //Optimizes performance by not creating a certificate for each https-enabled domain
                //Useful when certificate trust is not required by proxy clients
                //GenericCertificate = new X509Certificate2(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "genericcert.pfx"), "password")
            };

            //Fired when a CONNECT request is received
            explicitEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;

            //An explicit endpoint is where the client knows about the existence of a proxy
            //So client sends request in a proxy friendly manner
            _proxyServer.AddEndPoint(explicitEndPoint);
            _proxyServer.Start();

            //Transparent endpoint is useful for reverse proxy (client is not aware of the existence of proxy)
            //A transparent endpoint usually requires a network router port forwarding HTTP(S) packets or DNS
            //to send data to this endPoint
            var transparentEndPoint = new TransparentProxyEndPoint(IPAddress.Any, 8001, true)
            {
                //Generic Certificate hostname to use
                //when SNI is disabled by client
                GenericCertificateName = "google.com"
            };

            _proxyServer.AddEndPoint(transparentEndPoint);

            //proxyServer.UpStreamHttpProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };
            //proxyServer.UpStreamHttpsProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };

            foreach (var endPoint in _proxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

            //Only explicit proxies can be set as system proxy!
            _proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            _proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);

            //wait here (You can use something else as a wait function, I am using this as a demo)
            Console.Read();

            //Unsubscribe & Quit
            explicitEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
            _proxyServer.BeforeRequest -= OnRequest;
            _proxyServer.BeforeResponse -= OnResponse;
            _proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
            _proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;

            _proxyServer.Stop();
        }


        #endregion
        private async Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.WebSession.Request.RequestUri.Host;

            if (hostname.Contains("dropbox.com"))
            {
                //Exclude Https addresses you don't want to proxy
                //Useful for clients that use certificate pinning
                //for example dropbox.com

                await Task.Run(() => { e.DecryptSsl = false; });
            }
        }

        private List<IFilterEvent> events;

        public void AddEvent(IFilterEvent e)
        {
            events.Add(e);
        }

        public void ClearEvents()
        {
            events.Clear();
        }

        public async Task OnRequest(object sender, SessionEventArgs e)
        {
            foreach (var @event in events)
            {
                if (e.WebSession.Request.RequestUri.AbsoluteUri.Contains(@event.Url))
                {
                    string bodyString = await e.GetRequestBodyAsString();
                    await Task.Run(() =>
                    {
                        string msg = $"网址:{e.WebSession.Request.Url}" +
                                     $"\r\n协议：{(e.WebSession.IsHttps ? "Https" : "http")}" +
                                     $"\r\nMethod：{e.WebSession.Request.Method}" +
                                     $"\r\n PostData：{bodyString}\r\n\r\n";
                        Console.WriteLine(msg);
                    });
                }
            }
        }

        //Modify response
        public async Task OnResponse(object sender, SessionEventArgs e)
        {
            //read response headers
            var responseHeaders = e.WebSession.Response.Headers;

            //if (!e.ProxySession.Request.Host.Equals("medeczane.sgk.gov.tr")) return;
            if (e.WebSession.Request.Method == "GET" || e.WebSession.Request.Method == "POST")
            {
                if (e.WebSession.Response.StatusCode == 200)
                {
                    if (e.WebSession.Response.ContentType != null && e.WebSession.Response.ContentType.Trim().ToLower().Contains("text/html"))
                    {
                        byte[] bodyBytes = await e.GetResponseBody();
                        e.SetResponseBody(bodyBytes);

                        string body = await e.GetResponseBodyAsString();
                        e.SetResponseBodyString(body);
                    }
                }
            }

            if (e.UserData != null)
            {
                //access request from UserData property where we stored it in RequestHandler
                var request = (Request)e.UserData;
            }

        }

        /// Allows overriding default certificate validation logic
        public Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            //set IsValid to true/false based on Certificate Errors
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
                e.IsValid = true;

            return Task.FromResult(0);
        }

        /// Allows overriding default client certificate selection logic during mutual authentication
        public Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            //set e.clientCertificate to override
            return Task.FromResult(0);
        }
    }
}
