export default function ProductsLayout({ children }: { children: React.ReactNode }) {
    return (
      <div
        style={{
          backgroundColor: '#f0f8ff',
          border: '3px dashed blue',
          padding: '2rem',
          borderRadius: '8px',
        }}
      >
        <h2 style={{ color: 'blue', marginBottom: '1rem' }}>
          Products Section - Custom Layout
        </h2>
        {children}
      </div>
    );
    
  }