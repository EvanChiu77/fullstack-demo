import Link from 'next/link';

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <body style={{ margin: 0, padding: 0, fontFamily: 'sans-serif' }}>
        <header
          style={{
            backgroundColor: '#333',
            color: '#fff',
            padding: '1rem',
            textAlign: 'center',
          }}
        >
          <nav>
            <Link href="/" style={{ color: '#fff', marginRight: '1rem' }}>
              Home
            </Link>
            <Link href="/products" style={{ color: '#fff' }}>
              Products
            </Link>
          </nav>
        </header>
        <main style={{ padding: '1rem' }}>{children}</main>
        <footer
          style={{
            backgroundColor: '#333',
            color: '#fff',
            padding: '1rem',
            textAlign: 'center',
          }}
        >
          <p>Global Footer</p>
        </footer>
      </body>
    </html>
  );
}