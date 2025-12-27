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
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { IconDotsVertical, IconFilter, IconPlus } from '@tabler/icons-react';

const campaigns = [
  {
    name: 'Summer 2X',
    type: 'Multiplier',
    duration: '01–10 Jun 2025',
    status: 'ACTIVE',
  },
  {
    name: 'Weekend Cash',
    type: 'Cashback',
    duration: '03–12 Mar 2025',
    status: 'DRAFT',
  },
  {
    name: 'Diwali Gift',
    type: 'Coupon',
    duration: '01–20 Oct 2024',
    status: 'ENDED',
  },
];

export default function CampaignListPage() {
  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Campaigns</h1>
      <div className="flex justify-between items-center mb-4">
        <div></div> {/* This is a placeholder to push other elements to the right */}
        <div className="flex gap-2">
          <Button variant="outline">
            <IconFilter className="mr-2 h-4 w-4" />
            Filter
          </Button>
          <Button>
            <IconPlus className="mr-2 h-4 w-4" />
            New
          </Button>
        </div>
      </div>

      <div className="mb-4">
        <ToggleGroup type="single" defaultValue="all" variant="outline">
          <ToggleGroupItem value="all">All</ToggleGroupItem>
          <ToggleGroupItem value="draft">Draft</ToggleGroupItem>
          <ToggleGroupItem value="active">Active</ToggleGroupItem>
          <ToggleGroupItem value="paused">Paused</ToggleGroupItem>
          <ToggleGroupItem value="ended">Ended</ToggleGroupItem>
        </ToggleGroup>
      </div>

      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Duration</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="w-[50px]"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {campaigns.map((campaign) => (
              <TableRow key={campaign.name}>
                <TableCell className="font-medium">{campaign.name}</TableCell>
                <TableCell>{campaign.type}</TableCell>
                <TableCell>{campaign.duration}</TableCell>
                <TableCell>
                  <Badge
                    variant={
                      campaign.status === 'ACTIVE'
                        ? 'default'
                        : campaign.status === 'DRAFT'
                        ? 'secondary'
                        : 'destructive'
                    }
                  >
                    {campaign.status}
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
                      <DropdownMenuItem>Edit</DropdownMenuItem>
                      <DropdownMenuItem>Pause</DropdownMenuItem>
                      <DropdownMenuItem>Delete</DropdownMenuItem>
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
