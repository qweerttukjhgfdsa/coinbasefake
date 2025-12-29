using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoinbaseFake
{
    public partial class Form1 : Form
    {
        private readonly WebView2 webView = new();

        public Form1()
        {
            InitializeComponent();
            Text = "Coinbase";
            WindowState = FormWindowState.Maximized;
            webView.Dock = DockStyle.Fill;
            Controls.Add(webView);
            webView.CoreWebView2InitializationCompleted += (_, __) => _ = StartFakeAsync();
            _ = webView.EnsureCoreWebView2Async(null);
        }

        private async Task StartFakeAsync()
        {
            double usd = 69_420.42;
            string cfg = Path.Combine(Application.StartupPath, "balance.cfg");
            if (File.Exists(cfg) && double.TryParse(File.ReadLines(cfg).First(), out double v)) usd = v;

            await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                "localStorage.setItem('cbFake','{\"usd\":" + usd.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "}');" +
                "const _f=window.fetch;" +
                "window.fetch=async(...a)=>{" +
                "  const r=await _f(...a);" +
                "  if(a[0].includes('/api/v3/account')||a[0].includes('/wallet/v1/account')||a[0].includes('/api/v3/brokerage/accounts')){" +
                "     const d=await r.clone().json();" +
                "     if(d.balances)d.balances.forEach(x=>{if(x.currency==='USD')x.available='" + usd.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "';});" +
                "     if(d.data)d.data.totalUsdValue='" + usd.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "';" +
                "     return new Response(JSON.stringify(d),r);" +
                "  }" +
                "  return r;" +
                "};");

            webView.CoreWebView2.DOMContentLoaded += async (_, __) =>
            {
                await webView.CoreWebView2.ExecuteScriptAsync(
                    "setInterval(()=>{" +
                    "  const seq=['6','9','k'];" +
                    "  const ticker=document.querySelector('[data-testid=\"total-balance-header-balance\"]');" +
                    "  if(ticker){" +
                    "    const wheels=ticker.querySelectorAll('.PriceTickerChar__Values-sc-f6334e52-0');" +
                    "    wheels.forEach((wheel,idx)=>{" +
                    "      wheel.style.margin='0';" +
                    "      wheel.style.padding='0';" +
                    "      wheel.style.letterSpacing='0';" +
                    "      if(idx<seq.length){" +
                    "        const visible=wheel.querySelector('[aria-hidden=\"true\"][style*=\"user-select: auto\"]');" +
                    "        if(visible){" +
                    "          visible.textContent=seq[idx];" +
                    "          visible.style.transform='translateY(0)';" +
                    "        }" +
                    "      }else{" +
                    "        wheel.style.display='none';" +
                    "      }" +
                    "    });" +
                    "    ticker.setAttribute('data-value','USD 69k');" +
                    "  }" +
                    "  const cell=document.querySelector('[data-testid=\"user-cash-table-balance-content\"]');" +
                    "  if(cell)cell.textContent='USD 69 420,42';" +
                    "  const table=document.querySelector('[data-testid=\"table-column-value\"]');" +
                    "  if(table)table.textContent='USD 69 420,42';" +
                    "  const img=document.querySelector('img[src*=\"9d67b728b6c8f457717154b3a35f9ddc702eae7e76c4684ee39302c4d7fd0bb8\"]');" +
                    "  if(img)img.src='https://cryptologos.cc/logos/bitcoin-btc-logo.png';" +
                    "  const label=document.querySelector('span.headline-h15u0we0.headline-h1cz184y.headline-h1axe68h.headline-h1d8xh3w');" +
                    "  if(label&&label.textContent==='USDC')label.textContent='BTC';" +
                    "},3000);" +
                    "document.addEventListener('click',e=>{" +
                    "  const b=e.target.closest('button');" +
                    "  if(b&&(b.textContent||'').toLowerCase().includes('withdraw')){" +
                    "    e.preventDefault();e.stopImmediatePropagation();" +
                    "    alert('Daily withdrawal limit exceeded.');" +
                    "  }" +
                    "},true);");
            };

            webView.CoreWebView2.Navigate("https://www.coinbase.com");
        }
    }
}