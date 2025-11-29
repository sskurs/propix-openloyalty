import React from 'react';

interface ContentLayoutProps {
  title: string;
  children: React.ReactNode;
}

export function ContentLayout({ title, children }: ContentLayoutProps) {
  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">{title}</h1>
      {children}
    </div>
  );
}
