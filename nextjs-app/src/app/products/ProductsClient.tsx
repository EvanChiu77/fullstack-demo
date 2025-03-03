"use client"; //csr，這樣才能使用useState、useEffect等Hook
import { useState } from "react";
import { Product } from "./types";

//定義物件，並使用satisfies(Typescrpit 5新運算子)檢查它是否符合Interface
const sampleProduct = {
    id: 99,
    name: "Sample Product",
    price: 199.99,
    quantity: 2,
    // extraField: "Test for satisfies"
} satisfies Product;

export default function ProductsClient({ products: initialProducts }: { products: Product[] }) {
    // 使用useState管理產品清單狀態
    const [products, setProducts] = useState<Product[]>(initialProducts);
    // 使用useState管理錯誤訊息
    const [errorMsg, setErrorMsg] = useState("");

    // 當點擊按鈕時，呼叫此函數
    async function handlePurchase(id: number) {
        try {
            // 清除先前錯誤訊息
            setErrorMsg("");
            // 呼叫API
            const res = await fetch(`http://localhost:5231/api/purchase/${id}`, {
                method: "POST",
            });
            //回應非OK，取回錯誤訊息並拋出錯誤
            if (!res.ok) {
                const text = await res.text();
                throw new Error(text);
            }
            //取得更新後的產品資料，解析成Product型別
            const updatedProduct: Product = await res.json();
            //更新產品，把更新後的產品替換原資料
            setProducts((parameter) =>
                parameter.map((p) => (p.id === id ? updatedProduct : p))
            );
        } catch (error: unknown) {
            //使用Typescript的型別縮小處理錯誤
            if (error instanceof Error) {
                setErrorMsg(error.message);
            } else {
                setErrorMsg("An unknown error occurred");
            }
        }
    }

    return (
        <div>
            <h2>Sample Product Demo (使用 satisfies 檢查)</h2>
            <p>
                <strong>ID:</strong> {sampleProduct.id} |{" "}
                <strong>Name:</strong> {sampleProduct.name} |{" "}
                <strong>Price:</strong> ${sampleProduct.price} |{" "}
                <strong>Inventory:</strong> {sampleProduct.quantity}
            </p>
            <hr />
            <h2>產品清單</h2>
            {/* 若有錯誤訊息，則以紅色字體顯示 */}
            {errorMsg && <p style={{ color: "red" }}>{errorMsg}</p>}
            <ul>
                {products.map((product) => (
                    <li key={product.id}>
                        {product.name} - ${product.price} (庫存: {product.quantity})
                        {/* 購買按鈕，點擊後呼叫 handlePurchase */}
                        <button onClick={() => handlePurchase(product.id)}>購買</button>
                    </li>
                ))}
            </ul>
        </div>
    );
}