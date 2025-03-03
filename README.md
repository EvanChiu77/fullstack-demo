此專案使用 Next.js 15、ASP.NET Core C# 9、TypeScript 5、Redis 7 及 MSSQL 組成，透過 Docker Compose 統一配置，實現前後端分離、快取管理及完整的 API 功能。
目標是測試新版本技術的功能與優化，並在短時間內快速建立小型 Demo 專案。

##使用技術
| 技術             | 版本       | 實作項目                                   |
|------------------|------------|--------------------------------------------|
| **Next.js**      | 15         | App Router, 多層 Layout, Loading & Error 處理|
| **TypeScript**   | 5          | `satisfies` 運算子                         |
| **ASP.NET Core** | 9          | Minimal API, CRUD, Endpoint Filter          |
| **Redis**        | 7          | Caching, Cache Invalidation, Pub/Sub        |
| **MSSQL**        | 2022       | 資料庫管理與                               |
| **Docker**       | Compose    | 容器化佈署, Volume 設定                    |

總結
使用新版本技術後，開發效率與維護性顯著提升。  
Next.js 15 的 App Router 與自動錯誤處理機制，減少重複性程式碼。  
TypeScript 5 強型別的優勢在於除錯更高效，減少運行錯誤。  
Docker Compose 的統一佈署方式，解決了環境與版本控管的問題。  
Redis 的快取與即時通知功能，讓資料同步更迅速。



如何啟動專案
<br>git clone</br>
<br>cd fullstack-demo</br>
<br>docker-compose up  //(要有docker) </br>
