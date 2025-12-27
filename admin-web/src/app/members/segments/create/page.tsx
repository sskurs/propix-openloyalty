// Fresh implementation of Segments / Groups - Step 3
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import Link from 'next/link';

export default function CreateSegmentPage() {
  return (
    <div>
      <h1 className="text-3xl font-bold mb-8">Create New Segment</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8 max-w-4xl mx-auto">
          <Card className="text-center">
            <CardHeader>
              <CardTitle>Dynamic Segment</CardTitle>
              <CardDescription>Members are automatically added or removed based on a set of conditions.</CardDescription>
            </CardHeader>
            <CardContent>
              <Link href="/members/segments/create/dynamic">
                <Button>Create Dynamic Segment</Button>
              </Link>
            </CardContent>
          </Card>
          <Card className="text-center">
            <CardHeader>
              <CardTitle>Static Segment</CardTitle>
              <CardDescription>Manually manage members by importing a CSV file.</CardDescription>
            </CardHeader>
            <CardContent>
                <Button disabled>Create Static Segment (coming soon)</Button>
            </CardContent>
          </Card>
      </div>
    </div>
  );
}
