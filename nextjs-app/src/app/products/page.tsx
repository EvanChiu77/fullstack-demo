//此檔案用於在伺服器端抓取資料，然後傳遞給客戶端組件呈現 SSR運用

import { Product } from "./types";
import ProductsClient from "./ProductsClient"; 

//使用 async function 進行資料抓取（Server Component）
export default async function ProductsPage() {

  //模擬延遲3秒，以便觀察 loading.tsx
  await new Promise((resolve) => setTimeout(resolve, 3000));
  
  //直接使用fetch()於伺服器端執行，不需要getServerSideProps
  //後端API抓取產品資料，cache設定為no-store確保每次都重新抓取最新資料
  const res = await fetch('http://aspnet:80/api/products', {
    cache: 'no-store',
  });

  if (!res.ok) {
    throw new Error('Failed to fetch products');
  }

  const products: Product[] = await res.json();

  //將產品資料傳給ProductsClient客戶端的組件，處理畫面渲染與互動
  return <ProductsClient products={products} />;
}
