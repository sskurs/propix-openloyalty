'use client';

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { IconDotsVertical, IconFilter, IconSearch } from "@tabler/icons-react";
import { Input } from "@/components/ui/input";

// Mock data for the communication logs
const logs = [
  {
    id: 'log_1',
    dateTime: '2025-02-10 10:01',
    member: 'John Doe (MEM123)',
    channel: 'EMAIL',
    type: 'Campaign',
    subject: 'Weekend Double Points...',
    status: 'DELIVERED',
  },
  {
    id: 'log_2',
    dateTime: '2025-02-10 10:01',
    member: 'Sita Rao (MEM456)',
    channel: 'EMAIL',
    type: 'Campaign',
    subject: 'Weekend Double Points...',
    status: 'BOUNCED',
  },
  {
    id: 'log_3',
    dateTime: '2025-02-10 10:05',
    member: 'Raman (MEM789)',
    channel: 'SMS',
    type: 'System',
    subject: 'You earned 200 points',
    status: 'SENT',
  },
];

export default function CommunicationLogsPage() {
  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Communication Logs</h1>
      </div>

       <div className="flex justify-between items-center mb-4 gap-2">
            <div className="relative w-full max-w-sm">
                <IconSearch className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                <Input placeholder="Search by member or subject..." className="pl-8" />
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
              <TableHead>Date/Time</TableHead>
              <TableHead>Member</TableHead>
              <TableHead>Channel</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Subject / Preview</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="w-[50px]"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {logs.map((log) => (
              <TableRow key={log.id} className="hover:bg-muted/50">
                <TableCell>{log.dateTime}</TableCell>
                <TableCell className="font-medium">{log.member}</TableCell>
                <TableCell><Badge variant="outline">{log.channel}</Badge></TableCell>
                <TableCell>{log.type}</TableCell>
                <TableCell>{log.subject}</TableCell>
                <TableCell>
                  <Badge
                    variant={
                      log.status === 'DELIVERED' || log.status === 'SENT' ? 'success' : 
                      log.status === 'BOUNCED' ? 'destructive' : 'secondary'
                    }
                  >
                    {log.status}
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
                      <DropdownMenuItem>View Full Log</DropdownMenuItem>
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
