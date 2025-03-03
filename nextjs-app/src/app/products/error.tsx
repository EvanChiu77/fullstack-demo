'use client';

import { useEffect } from 'react';

export default function ProductsError({ error, reset }: { error: Error; reset: () => void }) {
  useEffect(() => {
    console.error('Error loading products:', error);
  }, [error]);

  return (
    <div style={{ color: 'red', padding: '1rem', textAlign: 'center' }}>
      <h3>Failed to load products.</h3>
      <p>{error.message}</p>
      <button onClick={() => reset()}>Try again</button>
    </div>
  );
}