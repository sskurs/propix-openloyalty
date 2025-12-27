'use client';

import { usePathname } from 'next/navigation';
import { useMemo } from 'react';

type BreadcrumbItem = {
  title: string;
  link: string;
};

// A simple regex to check if a string is a UUID
const isUuid = (str: string) => {
    const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    return uuidRegex.test(str);
};


export function useBreadcrumbs() {
  const pathname = usePathname();

  const breadcrumbs = useMemo(() => {
    const segments = pathname.split('/').filter(Boolean);
    
    // Start with a root breadcrumb
    const generatedBreadcrumbs: BreadcrumbItem[] = [{ title: 'Dashboard', link: '/' }];

    segments.slice(1).forEach((segment, index) => {
      const path = `/${segments.slice(0, index + 2).join('/')}`;
      
      // If the segment is a UUID, display 'Details' instead
      const title = isUuid(segment) 
        ? 'Details' 
        : segment.charAt(0).toUpperCase() + segment.slice(1);

      generatedBreadcrumbs.push({ title, link: path });
    });

    return generatedBreadcrumbs;
  }, [pathname]);

  return breadcrumbs;
}
