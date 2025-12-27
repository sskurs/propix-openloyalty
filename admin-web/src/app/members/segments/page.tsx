'use client';
import { useEffect, useState } from 'react';
import { getSegments, updateSegmentStatus } from '@/api/segmentsApi';
import { Segment } from '@/api/segmentsApi';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
  DropdownMenuSeparator
} from '@/components/ui/dropdown-menu';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { IconDotsVertical, IconFilter, IconPlus, IconSearch } from '@tabler/icons-react';
import Link from 'next/link';
import { format } from 'date-fns';
import { toast } from 'sonner';

export default function SegmentsPage() {
  const [segments, setSegments] = useState<Segment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchSegments = async () => {
    try {
      setLoading(true);
      const data = await getSegments();
      setSegments(data);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch segments.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSegments();
  }, []);

  const handleStatusToggle = (segment: Segment) => {
    const newStatus = segment.status === 'ACTIVE' ? 'DRAFT' : 'ACTIVE';
    const promise = updateSegmentStatus(segment.id, newStatus)
      .then(() => fetchSegments()); // Re-fetch the list on success

    toast.promise(promise, {
      loading: 'Updating status...',
      success: `Segment "${segment.name}" has been ${newStatus.toLowerCase()}`,
      error: (err) => err.message || 'Failed to update status.'
    });
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Segments</h1>
        <div className="flex gap-2">
          <Link href="/members/segments/create/dynamic">
            <Button>
              <IconPlus className="mr-2 h-4 w-4" />
              Add Segment
            </Button>
          </Link>
        </div>
      </div>

       <div className="flex justify-between items-center mb-4 gap-2">
            <div className="relative w-full max-w-sm">
                <IconSearch className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                <Input placeholder="Search by name..." className="pl-8" />
            </div>
            <Button variant="outline">
                <IconFilter className="mr-2 h-4 w-4" />
                Filter
            </Button>
       </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Members</TableHead>
              <TableHead>Avg. Transaction</TableHead>
              <TableHead>Avg. # Transactions</TableHead>
              <TableHead>Avg. Spending</TableHead>
              <TableHead>Created On</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="w-[50px]"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading && <TableRow><TableCell colSpan={8} className="text-center">Loading...</TableCell></TableRow>}
            {error && <TableRow><TableCell colSpan={8} className="text-center text-red-500">{error}</TableCell></TableRow>}
            {!loading && !error && segments.map((segment) => (
              <TableRow key={segment.id} className="hover:bg-muted/50">
                <TableCell className="font-medium">{segment.name}</TableCell>
                <TableCell>0</TableCell>
                <TableCell>₹0.00</TableCell>
                <TableCell>0.0</TableCell>
                <TableCell>₹0.00</TableCell>
                <TableCell>{format(new Date(segment.createdAt), 'dd MMM yyyy')}</TableCell>
                 <TableCell>
                  <Badge
                    variant={
                      segment.status === 'ACTIVE' ? 'success' : 'secondary'
                    }
                  >
                    {segment.status}
                  </Badge>
                </TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <span className="sr-only">Open menu</span>
                        <IconDotsVertical className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                        <DropdownMenuItem asChild><Link href={`/members/segments/${segment.id}/members`}>View Members</Link></DropdownMenuItem>
                        <DropdownMenuItem asChild><Link href={`/members/segments/${segment.id}/edit`}>Edit Segment</Link></DropdownMenuItem>
                        <DropdownMenuItem onClick={() => handleStatusToggle(segment)}>{segment.status === 'ACTIVE' ? 'Deactivate' : 'Activate'}</DropdownMenuItem>
                        <DropdownMenuItem>Download Members (CSV)</DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem className="text-red-600 hover:!text-red-600 hover:!bg-red-100">Remove Segment</DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
