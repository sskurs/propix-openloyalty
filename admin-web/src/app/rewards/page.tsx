'use client';
import { useState } from 'react';
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
} from '@/components/ui/dropdown-menu';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { IconDotsVertical, IconFilter, IconPlus } from '@tabler/icons-react';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Checkbox } from '@/components/ui/checkbox';

// Mock data for the rewards catalog
const rewards = [
  {
    name: 'Free Coffee',
    points: 100,
    type: 'Coupon',
    validity: '30 days',
    status: 'ACTIVE',
  },
  {
    name: '5% Off Code',
    points: 200,
    type: 'Coupon',
    validity: '15 days',
    status: 'PAUSED',
  },
  {
    name: 'Flight Miles',
    points: 500,
    type: 'Partner',
    validity: '60 days',
    status: 'DRAFT',
  },
];

const CreateRewardForm = ({ onCancel }: { onCancel: () => void }) => (
  <div>
    <h1 className="text-3xl font-bold mb-8">Create New Reward</h1>
    <Card className="max-w-2xl mx-auto">
      <CardHeader><CardTitle>Reward Definition</CardTitle></CardHeader>
      <CardContent className="space-y-6">
        <div className="space-y-2"><Label htmlFor="name">Reward Name</Label><Input id="name" placeholder="e.g., Free Coffee" required /></div>
        <div className="space-y-2"><Label htmlFor="type">Type</Label><Select required><SelectTrigger id="type"><SelectValue placeholder="Select reward type" /></SelectTrigger><SelectContent><SelectItem value="coupon">Coupon</SelectItem><SelectItem value="partner">Partner</SelectItem><SelectItem value="physical">Physical Product</SelectItem></SelectContent></Select></div>
        <div className="space-y-2"><Label htmlFor="points">Points Cost</Label><Input id="points" type="number" placeholder="e.g., 100" required /></div>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4"><div className="space-y-2"><Label htmlFor="validity-days">Validity (Days)</Label><Input id="validity-days" type="number" placeholder="e.g., 30" /></div><div className="space-y-2"><Label htmlFor="validity-range">Or Date Range</Label><div className="flex space-x-2"><Input id="validity-start" type="date" /><Input id="validity-end" type="date" /></div></div></div>
        <div className="space-y-4"><Label>Visibility Rules (Optional)</Label><div className="grid grid-cols-2 gap-4"><div className="space-y-2"><Label className="text-sm">Tiers</Label><div className="flex items-center space-x-2"><Checkbox id="tier-gold" /><Label htmlFor="tier-gold">Gold</Label></div><div className="flex items-center space-x-2"><Checkbox id="tier-silver" /><Label htmlFor="tier-silver">Silver</Label></div></div><div className="space-y-2"><Label className="text-sm">Tags</Label><div className="flex items-center space-x-2"><Checkbox id="tag-vip" /><Label htmlFor="tag-vip">VIP</Label></div></div></div></div>
      </CardContent>
      <CardFooter className="flex justify-end gap-2">
          <Button variant="outline" onClick={onCancel}>Cancel</Button>
          <Button variant="secondary">Save Draft</Button>
          <Button>Activate</Button>
      </CardFooter>
    </Card>
  </div>
);

export default function RewardsPage() {
  const [isCreating, setIsCreating] = useState(false);

  if (isCreating) {
    return <CreateRewardForm onCancel={() => setIsCreating(false)} />;
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Rewards</h1>
        <div className="flex gap-2">
          <Button variant="outline"><IconFilter className="mr-2 h-4 w-4" />Filter</Button>
          <Button onClick={() => setIsCreating(true)}><IconPlus className="mr-2 h-4 w-4" />New</Button>
        </div>
      </div>
      <div className="rounded-md border">
        <Table>
          <TableHeader><TableRow><TableHead>Name</TableHead><TableHead>Points Req</TableHead><TableHead>Type</TableHead><TableHead>Validity</TableHead><TableHead>Status</TableHead><TableHead className="w-[50px]"></TableHead></TableRow></TableHeader>
          <TableBody>
            {rewards.map((reward) => (
              <TableRow key={reward.name}>
                <TableCell className="font-medium">{reward.name}</TableCell>
                <TableCell>{reward.points}</TableCell>
                <TableCell>{reward.type}</TableCell>
                <TableCell>{reward.validity}</TableCell>
                <TableCell><Badge variant={reward.status === 'ACTIVE' ? 'success' : reward.status === 'PAUSED' ? 'secondary' : 'outline'}>{reward.status}</Badge></TableCell>
                <TableCell><DropdownMenu><DropdownMenuTrigger asChild><Button variant="ghost" className="h-8 w-8 p-0"><span className="sr-only">Open menu</span><IconDotsVertical className="h-4 w-4" /></Button></DropdownMenuTrigger><DropdownMenuContent align="end"><DropdownMenuItem>Edit</DropdownMenuItem><DropdownMenuItem>Pause</DropdownMenuItem><DropdownMenuItem>Delete</DropdownMenuItem></DropdownMenuContent></DropdownMenu></TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
