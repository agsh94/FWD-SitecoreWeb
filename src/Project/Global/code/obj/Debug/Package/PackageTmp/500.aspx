<% Response.StatusCode = 500; %>
<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="ie=edge" />
    <title>Sorry for the unexpected problem | FWD Thailand</title>
    <link rel="shortcut icon" href="/-/media/global/images/icons/favicon.ico" type="image/x-icon" />
    <style type="text/css">
      body {
        margin: 0;
      }

      /* en font*/
      @font-face {
        font-family: 'FWDCircularWeb-Black';
        src: url('fonts/en/FWDCircularWeb-Black.eot');
        src: url('fonts/en/FWDCircularWeb-Black.woff2') format('woff2'),
          url('fonts/en/FWDCircularWeb-Black.woff') format('woff');
        font-display: swap;
        font-weight: normal;
        font-style: normal;
      }

      @font-face {
        font-family: 'FWDCircularWeb-Book';
        src: url('fonts/en/FWDCircularWeb-Book.eot');
        src: url('fonts/en/FWDCircularWeb-Book.woff2') format('woff2'),
          url('fonts/en/FWDCircularWeb-Book.woff') format('woff');
        font-display: swap;
        font-weight: normal;
        font-style: normal;
      }

      /* thai font*/
      @font-face {
        font-family: 'NotoSansThai-Regular';
        src: url('fonts/th/NotoSansThai-Regular.eot');
        src: url('fonts/th/NotoSansThai-Regular.eot?#iefix') format('embedded-opentype'),
          url('fonts/th/NotoSansThai-Regular.woff2') format('woff2'),
          url('fonts/th/NotoSansThai-Regular.woff') format('woff'),
          url('fonts/th/NotoSansThai-Regular.ttf') format('truetype'),
          url('fonts/th/NotoSansThai-Regular.svg#NotoSansThai-Regular') format('svg');
        font-weight: normal;
        font-style: normal;
        font-display: swap;
      }

      @font-face {
        font-family: 'NotoSansThai-SemBd';
        src: url('fonts/th/NotoSansThai-SemiBold.eot');
        src: url('fonts/th/NotoSansThai-SemiBold.eot?#iefix') format('embedded-opentype'),
          url('fonts/th/NotoSansThai-SemiBold.woff2') format('woff2'),
          url('fonts/th/NotoSansThai-SemiBold.woff') format('woff'),
          url('fonts/th/NotoSansThai-SemiBold.ttf') format('truetype'),
          url('fonts/th/NotoSansThai-SemiBold.svg#NotoSansThai-SemiBold') format('svg');
        font-weight: 600;
        font-style: normal;
        font-display: swap;
      }

      /**
    * Japanese Fonts
    */
      @font-face {
        font-family: 'NotoSansJP';
        font-style: normal;
        font-weight: 300;
        font-display: swap;
        src: url(//fonts.gstatic.com/ea/notosansjapanese/v6/NotoSansJP-DemiLight.woff2)
            format('woff2'),
          url(//fonts.gstatic.com/ea/notosansjapanese/v6/NotoSansJP-DemiLight.woff) format('woff'),
          url(//fonts.gstatic.com/ea/notosansjapanese/v6/NotoSansJP-DemiLight.otf)
            format('opentype');
      }

      @font-face {
        font-family: 'NotoSansJP';
        font-style: normal;
        font-weight: 700;
        font-display: swap;
        src: url(//fonts.gstatic.com/ea/notosansjapanese/v6/NotoSansJP-Bold.woff2) format('woff2'),
          url(//fonts.gstatic.com/ea/notosansjapanese/v6/NotoSansJP-Bold.woff) format('woff'),
          url(//fonts.gstatic.com/ea/notosansjapanese/v6/NotoSansJP-Bold.otf) format('opentype');
      }

      /* Chinese */
      @font-face {
        font-family: 'NotoSansTC';
        font-style: normal;
        font-weight: 300;
        font-display: swap;
        src: url(https://fonts.gstatic.com/s/notosanstc/v10/-nF7OG829Oofr2wohFbTp9iFPA.woff)
          format('woff');
      }

      @font-face {
        font-family: 'NotoSansTC';
        font-style: normal;
        font-weight: 700;
        font-display: swap;
        src: url(https://fonts.gstatic.com/s/notosanstc/v10/-nFkOG829Oofr2wohFbTp9i9ywIvCA.woff)
          format('woff');
      }

      .headerContent {
        margin-left: 71px;
        padding: 20px 32px 19px;
        box-shadow: 0 2px 4px 0 rgba(0, 0, 0, 0.1);
        border-bottom-left-radius: 16px;
      }

      .fwd-logo {
        width: 95px;
        height: 33px;
      }

      .main {
        margin: 0 auto;
        text-align: center;
        padding-left: 71px;
        padding-right: 71px;
      }

      .fwdContentTab {
        padding: 96px 0;
      }

      .en-heading {
        font-family: 'FWDCircularWeb-Black', sans-serif;
      }

      .en-regular {
        font-family: 'FWDCircularWeb-Book', sans-serif;
      }

      .th-heading {
        font-family: 'NotoSansThai-SemBd', sans-serif;
      }

      .th-regular {
        font-family: 'NotoSansThai-Regular', sans-serif;
      }

      .ja-heading {
        font-family: 'NotoSansJP', sans-serif;
      }

      .ja-regular {
        font-family: 'NotoSansJP', sans-serif;
      }

      .tc-heading {
        font-family: 'NotoSansTC', sans-serif;
      }

      .tc-regular {
        font-family: 'NotoSansTC', sans-serif;
      }

      .errorTitle {
        color: #183028;
        font-size: 25px;
        font-weight: 700;
        letter-spacing: 0;
        line-height: 31px;
        text-align: center;
        margin: 0 0 8px;
      }

      .errorDescription {
        color: #183028;
        font-size: 16px;
        font-weight: 300;
        letter-spacing: 0;
        line-height: 24px;
        text-align: center;
        margin: 0;
      }

      .callSupport {
        color: #183028;
      }

      .tab {
        border-bottom: 1px solid #dbdfe1;
        margin-bottom: 64px;
        display: flex;
        align-items: center;
        justify-content: center;
      }

      .tab button {
        color: #183028;
        font-family: 'FWDCircularWeb-Black', sans-serif;
        font-weight: 700;
        border: none;
        background-color: transparent;
        outline: none;
        cursor: pointer;
        padding: 4px 16px;
        margin: 0;
        transition: 0.3s;
        font-size: 20px;
        line-height: 25px;
        border-bottom: 4px solid transparent;
        display: block;
      }

      .tab button.active {
        color: #e87722;
        border-bottom: 4px solid #e87722;
      }

      .tabcontent {
        display: none;
        padding: 0;
        -webkit-animation: fadeEffect 1s;
        animation: fadeEffect 1s;
        max-width: 600px;
        margin: 0 auto;
      }

      @media only screen and (max-width: 600px) {
        .main {
          max-width: 600px;
          padding: 0;
        }

        .headerContent {
          padding: 18px 16px;
          margin-left: 56px;
        }

        .fwd-logo {
          width: 56px;
          height: 20px;
        }

        .fwdContentTab {
          padding: 64px 16px;
        }

        .tab {
          margin-bottom: 48px;
        }

        .tab button {
          font-size: 16px;
          line-height: 20px;
        }
      }

      /* Fade in tabs */
      @-webkit-keyframes fadeEffect {
        from {
          opacity: 0;
        }

        to {
          opacity: 1;
        }
      }

      @keyframes fadeEffect {
        from {
          opacity: 0;
        }

        to {
          opacity: 1;
        }
      }
    </style>
  </head>

  <body>
    <div class="container">
      <!-- Header section start here -->
      <header class="header">
        <div class="headerContent">
          <div class="fwd-logo">
            <svg
              class="MuiSvgIcon-root MuiSvgIcon-root svglogo"
              focusable="false"
              viewBox="0 0 105 36"
              aria-hidden="true"
              role="presentation"
              data-mui-test="FWDLogoIcon"
              data-locator="Header-Logo__img"
            >
              <path
                d="m85.0647368 16.8813014c.2339474.2394642.3647369.553646.3647369.8886508 0 .332555-.1307895.6510239-.3647369.8898756l-6.2158772 6.1452248c-.2468421.2388517-.5728947.3680766-.9001754.3680766-.1633333 0-.3291228-.0275598-.4881579-.0967656-.472807-.1947559-.7810526-.6516363-.7810526-1.1593492v-12.2922871c0-.5107751.3082456-.9664306.7810526-1.1648613.4771053-.1929186 1.0229825-.0839043 1.3883333.2737608z"
                fill="#e87722"
              ></path>
              <path
                d="m46.1005263.88926316c-.7386842.59467942-1.2642982 1.29898564-1.5903509 2.12026794-.3242105.81699522-.6821929 2.00390431-1.1009649 3.56991388l-.3278947 1.22855502-4.0403509 15.8530144-4.708421-20.27789478c-.4003509-1.93776077-1.4890351-3.13385646-4.1846492-3.13385646h-28.77491224c-1.32263158 0-1.36929825 1.39330143-1.37298246 1.4049378v32.86047844s.02210526.6987943.6981579.6987943h2.67350877c1.92008772 0 4.37438596-1.2346794 4.37438596-4.6398469v-10.0103349h10.07631577c3.8432456 0 4.7790351-2.7363828 4.7790351-4.2925933v-.5781436c0-.552421-.1872807-.8874258-.8553509-.8874258h-13.99999997v-8.29182776h19.79280697l5.9727193 22.93833496c.1829825.6865454 1.2280702 4.3164784 1.2280702 4.3164784.402193 1.2726508.3579825 1.4576077 1.4448246 1.4576077h2.4199123c2.9731579 0 3.9998245-1.8269091 4.5248245-3.1160957.2640351-.6987943.4513158-1.0062392.8756141-2.5820479l5.2659649-19.77385641 5.5699123 19.67525361c.1811403.6871579 1.1531578 4.331177 1.1531578 4.331177.400965 1.2683636.3604386 1.4631196 1.4478948 1.4631196h2.4174561c2.975 0 4.0035088-1.8287464 4.5291228-3.1258947.2572807-.6987943.5170176-1.3669665.943772-2.9391005l5.4753508-22.62537798h16.9547369c1.4577193 0 2.6311403.12677512 3.6878947.44585646 1.3668421.40972249 2.5826316 1.43188517 3.5970176 3.03157892.9824561 1.5684594 1.5442982 4.3146412 1.5700877 7.725933-.0018421 3.8448996-1.155 7.2580288-3.2967544 9.1026986-.4771053.429933-1.0371053.7631005-1.7008772 1.0062392-.6766667.2486508-1.330614.4054354-1.9507895.4679043-.6680701.0698182-1.5522807.2045551-2.7152631.2143541h-.016579l-3.6977193.0061244h-5.3120175l-.0264035-.0061244c-1.1488597 0-1.1850877.6651101-1.1850877.9517321v1.3755406c0 2.0014546 1.6566666 4.3023924 4.9835087 4.3023924h.1148246l6.4535088-.0036747c1.6425438-.0165358 3.116228-.1365741 4.4370175-.3613397 1.3281579-.2308899 2.5783333-.6106028 3.7419298-1.1624115 1.1635965-.5444593 2.2381579-1.2671387 3.2163158-2.1588516 1.236053-1.1562871 2.244912-2.4632345 3.031491-3.9245168.777369-1.4618947 1.350877-3.0903732 1.711316-4.8829856.340175-1.6927847.509035-2.8974546.509035-4.9050335v-.0422584c-.056491-6.5010526-1.982719-11.1225263-5.8210525-14.34886123-1.4718421-1.26713875-3.1303509-2.13435406-4.9540351-2.58817225-1.6947368-.42564593-3.6958772-.63448803-5.9825439-.63448803h-20.5419298c-1.1273684.0122488-2.8165789.59222966-3.4662281 3.23858373l-4.4566666 20.14744498-4.7366667-15.79850718v.01163636l-.3180702-1.1642488c-.4568421-1.68421053-.8252631-2.89316747-1.0991228-3.62380862-.2757017-.72880382-.7607894-1.40371292-1.4736842-2.02778947-.7024561-.61856459-1.6965789-.92784689-2.9731579-.92784689-1.2661403 0-2.258421.29519617-2.9909649.88926316"
                fill="#25484c"
              ></path>
            </svg>
          </div>
        </div>
      </header>
      <!-- Header section end here -->
      <!-- Content section start here -->
      <div class="content">
        <div class="main">
          <div id="tabContainer" class="fwdContentTab">
            <div id="tabs" class="tab"></div>
          </div>
        </div>
      </div>
      <!-- Content section end here -->
    </div>
    <script>
      /*
       * Market Specific Regex
       */
      var mktRegex = {
        //localhost
        localhost: '^localhost',
        //thailand
        th: '-th|.th$',
        // Hong Kong
        hk: '-hk|.hk$',
        // Indonesia
        id: '-id|.id$',
        // Japan
        jp: '-jp|.jp$',
        // Go
        go:
          '-go|.go$|^uat-www.fwd.com|^uat-stage-www.fwd.com|^www.fwd.com|^fwd.com|^prd-stage-www.fwd.com',
      };
      /*
       * Market Specific Tab Content
       */
      var tabcontents = {
        //localhost
        localhost: {
          en:
            '<div id="ctx-en" class="tabcontent">' +
            '<h2 class="errorTitle en-heading">' +
            'Sorry for the unexpected error' +
            '</h2>' +
            '<p class="errorDescription en-regular">' +
            'An error has occurred and we’re trying to fix the problem. ' +
            'In the meantime, if you need help from us, please call <a class="callSupport" href="tel://1351">1351</a>.' +
            '</p>' +
            '</div>',
        },
        //thailand
        th: {
          th:
            '<div id="ctx-th" class="tabcontent">' +
            '<h2 class="errorTitle th-heading">ขออภัย! มีความผิดพลาดเกิดขึ้น</h2>' +
            '<p class="errorDescription th-regular">' +
            'โปรดรอสักครู่ หรือกดรีเฟรช หากไม่สามารถแก้ไขได้ กรุณาติดต่อ <a class="callSupport" href="tel://1351">1351</a>' +
            '</p>' +
            '</div>',
          en:
            '<div id="ctx-en" class="tabcontent">' +
            '<h2 class="errorTitle en-heading">' +
            'Sorry for the unexpected error' +
            '</h2>' +
            '<p class="errorDescription en-regular">' +
            'An error has occurred and we’re trying to fix the problem. ' +
            'In the meantime, if you need help from us, please call <a class="callSupport" href="tel://1351">1351</a>.' +
            '</p>' +
            '</div>',
        },
        // Hong Kong
        hk: {
          tc:
            '<div id="ctx-tc" class="tabcontent">' +
            '<h2 class="errorTitle tc-heading">' +
            '很抱歉，發生了意外的錯誤' +
            '</h2>' +
            '<p class="errorDescription tc-regular">' +
            '我們正在努力修正。' +
            '在此期間，如果你需要協助，請打<a class="callSupport" href="tel://31233123">3123 3123</a>與我們聯絡。' +
            '</p>' +
            '</div>',
          en:
            '<div id="ctx-en" class="tabcontent">' +
            '<h2 class="errorTitle en-heading">' +
            'Sorry for the unexpected error' +
            '</h2>' +
            '<p class="errorDescription en-regular">' +
            'We’re working hard to fix it.<br>' +
            'In the meantime, if you need help from us, please call <a class="callSupport" href="tel://31233123">3123 3123</a>.' +
            '</p>' +
            '</div>',
        },
        // Go
        go: {
          en:
            '<div id="ctx-en" class="tabcontent">' +
            '<h2 class="errorTitle en-heading">' +
            'Sorry for the unexpected problem' +
            '</h2>' +
            '<p class="errorDescription en-regular">' +
            'We’re working hard to fix it. Please try again later.' +
            '</p>' +
            '</div>',
        },
        // Indonesia
        id: {
          id:
            '<div id="ctx-id" class="tabcontent">' +
            '<h2 class="errorTitle en-heading">Maaf, ada masalah yang tidak terduga di server kami</h2>' +
            '<p class="errorDescription en-regular">' +
            'Mohon menunggu, kami sedang melakukan perbaikan. Jika kamu membutuhkan bantuan segera dari kami, silakan hubungi <a class="callSupport" href="tel://1500525">1500525</a>' +
            '</p>' +
            '</div>',
          en:
            '<div id="ctx-en" class="tabcontent">' +
            '<h2 class="errorTitle en-heading">' +
            'Sorry for the unexpected problem' +
            '</h2>' +
            '<p class="errorDescription en-regular">' +
            'We’re working hard to fix it.<br>' +
            'In the meantime, if you need help from us, please call <a class="callSupport" href="tel://1500525">1500525</a>.' +
            '</p>' +
            '</div>',
        },
        // Japan
        jp: {
          ja:
            '<div id="ctx-ja" class="tabcontent">' +
            '<h2 class="errorTitle en-heading">' +
            '500 Internal Server Error' +
            '</h2>' +
            '<p class="errorDescription ja-regular">' +
            'サーバーで問題が発生しているためページを表示できません。しばらく時間をおいてからお試しください。<br>' +
            '下記お手続きは自動応答 <a class="callSupport" href="tel://0120622211">0120-622-211</a>で対応できますのでご利用ください。<br>' +
            '控除証明書再発行・解約(全部解約)・住所変更<br>' +
            '保険料振込口座変更・クレジットカード変更<br>' +
            '</p>' +
            '</div>',
        },
      };
      /*
       * Market Specific Page Title
       */
      var pageTitle = {
        //localhost
        localhost: 'Sorry for the unexpected error | FWD Thailand',
        //thailand
        th: 'ขออภัย! มีความผิดพลาดเกิดขึ้น | FWD Thailand',
        // Hong Kong
        hk: '很抱歉，發生了意外的錯誤 | FWD HongKong',
        // Go
        go: 'Sorry for the unexpected problem | FWD Group',
        // Indonesia
        id: 'Maaf, ada masalah yang tidak terduga di server kami | FWD Indonesia',
        // Japan
        jp: '500 内部サーバーエラー',
      };

      /*
       * Generate Market Specific Tabs
       */
      function generateTabBtn(lang) {
        return (
          '<button id="' +
          lang +
          '" class="tablinks" onclick="openTab(event, id)">' +
          lang.toUpperCase() +
          '</button>'
        );
      }

      /*
       * Based on hostname render the tab and tab content
       */
      document.addEventListener('DOMContentLoaded', function(event) {
        var hostname = window.location.hostname;
        if (hostname) {
          var market = 'localhost';
          Object.keys(mktRegex).some((item, index) => {
            var replace = mktRegex[item];
            var re = new RegExp(replace, 'g');
            if (hostname.match(re)) {
              market = item;
              return true;
            }
          });
          document.title = pageTitle[market];
          var tabs = document.getElementById('tabs');
          var tabContainer = document.getElementById('tabContainer');

          Object.keys(tabcontents[market]).forEach(lang => {
            tabs.innerHTML = tabs.innerHTML + generateTabBtn(lang);
          });
          if (Object.keys(tabcontents[market]).length == 1) {
            tablinks = document.getElementById('tabs').style.visibility = 'hidden';
          }
          Object.values(tabcontents[market]).forEach(content => {
            tabContainer.innerHTML = tabContainer.innerHTML + content;
          });
          document.querySelectorAll('.tablinks')[0].click();
        }
      });

      /*
       * Handle Tab change event
       */
      function openTab(evt, locale) {
        var i, tabcontent, tablinks;
        tabcontent = document.getElementsByClassName('tabcontent');
        for (i = 0; i < tabcontent.length; i++) {
          tabcontent[i].style.display = 'none';
        }
        tablinks = document.getElementsByClassName('tablinks');
        for (i = 0; i < tablinks.length; i++) {
          tablinks[i].className = tablinks[i].className.replace(' active', '');
        }
        document.getElementById('ctx-' + locale).style.display = 'block';
        evt.currentTarget.className += ' active';
      }
    </script>
  </body>
</html>
