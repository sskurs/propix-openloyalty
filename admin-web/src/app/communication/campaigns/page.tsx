'use client';

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger, DropdownMenuSeparator } from "@/components/ui/dropdown-menu";
import { IconDotsVertical, IconFilter, IconPlus, IconSearch } from "@tabler/icons-react";
import { Input } from "@/components/ui/input";
import Link from "next/link";

// Mock data for the communication campaigns list
const campaigns = [
  {
    id: 'comm_1',
    name: 'Weekend Offer',
    channel: 'EMAIL',
    type: 'One-off',
    status: 'SCHEDULED',
    schedule: '2025-02-15 10:00',
  },
  {
    id: 'comm_2',
    name: 'Welcome Series',
    channel: 'EMAIL',
    type: 'Journey',
    status: 'ACTIVE',
    schedule: 'Event-based',
  },
  {
    id: 'comm_3',
    name: 'Points Reminder',
    channel: 'SMS',
    type: 'One-off',
    status: 'COMPLETED',
    schedule: 'Sent: 2025-01-05',
  },
  {
    id: 'comm_4',
    name: 'Draft Campaign',
    channel: 'SMS',
    type: 'One-off',
    status: 'DRAFT',
    schedule: '-',
  },
];

export default function CommunicationCampaignsPage() {
  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Communication Campaigns</h1>
        <div className="flex gap-2">
          <Link href="/communication/campaigns/create">
            <Button>
              <IconPlus className="mr-2 h-4 w-4" />
              New Campaign
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
              <TableHead>Channel</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Scheduled / Sent</TableHead>
              <TableHead className="w-[50px]"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {campaigns.map((campaign) => (
              <TableRow key={campaign.id} className="hover:bg-muted/50">
                <TableCell className="font-medium">{campaign.name}</TableCell>
                <TableCell><Badge variant="outline">{campaign.channel}</Badge></TableCell>
                <TableCell>{campaign.type}</TableCell>
                <TableCell>
                  <Badge
                    variant={
                      campaign.status === 'ACTIVE' || campaign.status === 'SCHEDULED' ? 'success' :
                      campaign.status === 'COMPLETED' ? 'secondary' : 'destructive' // Corrected: 'warning' is not a valid variant
                    }
                  >
                    {campaign.status}
                  </Badge>
                </TableCell>
                <TableCell>{campaign.schedule}</TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <span className="sr-only">Open menu</span>
                        <IconDotsVertical className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem>View Details</DropdownMenuItem>
                      <DropdownMenuItem>Edit</DropdownMenuItem>
                      <DropdownMenuItem>Duplicate</DropdownMenuItem>
                      <DropdownMenuItem>{campaign.status === 'SCHEDULED' ? 'Cancel' : (campaign.status === 'ACTIVE' ? 'Pause' : 'Re-activate')}</DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem className="text-red-600 hover:!text-red-600 hover:!bg-red-100">Delete</DropdownMenuItem>
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
