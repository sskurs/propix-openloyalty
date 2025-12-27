'use client';

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { IconDotsVertical, IconFilter, IconPlus, IconSearch } from "@tabler/icons-react";
import { Input } from "@/components/ui/input";
import Link from "next/link";

// Mock data for the templates list
const templates = [
  {
    id: 'tpl_1',
    name: 'Welcome Email',
    channel: 'EMAIL',
    type: 'Marketing',
    status: 'ACTIVE',
    updatedOn: '2025-01-10',
  },
  {
    id: 'tpl_2',
    name: 'Points Earned SMS',
    channel: 'SMS',
    type: 'System',
    status: 'ACTIVE',
    updatedOn: '2025-02-02',
  },
  {
    id: 'tpl_3',
    name: 'Tier Upgrade',
    channel: 'EMAIL',
    type: 'System',
    status: 'DRAFT',
    updatedOn: '2025-02-05',
  },
];

export default function TemplatesPage() {
  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Templates</h1>
        <div className="flex gap-2">
          <Link href="/communication/templates/create">
            <Button>
              <IconPlus className="mr-2 h-4 w-4" />
              New Template
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
              <TableHead>Updated On</TableHead>
              <TableHead className="w-[50px]"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {templates.map((template) => (
              <TableRow key={template.id} className="hover:bg-muted/50">
                <TableCell className="font-medium">{template.name}</TableCell>
                <TableCell><Badge variant="outline">{template.channel}</Badge></TableCell>
                <TableCell>{template.type}</TableCell>
                <TableCell>
                  <Badge variant={template.status === 'ACTIVE' ? 'success' : 'secondary'}>{template.status}</Badge>
                </TableCell>
                <TableCell>{template.updatedOn}</TableCell>
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
                      <DropdownMenuItem>Duplicate</DropdownMenuItem>
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
