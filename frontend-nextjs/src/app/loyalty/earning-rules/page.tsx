'use client';

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger, DropdownMenuSeparator } from "@/components/ui/dropdown-menu";
import { IconDotsVertical, IconFilter, IconPlus, IconSearch } from "@tabler/icons-react";
import { Input } from "@/components/ui/input";
import Link from "next/link";

// Mock data for the earning rules list
const earningRules = [
  {
    id: 'rule_1',
    name: 'Base Spend Rule',
    category: 'Base',
    event: 'PURCHASE_COMPLETED',
    status: 'ACTIVE',
    validity: 'Always',
    priority: 100,
  },
  {
    id: 'rule_2',
    name: 'Weekend Booster',
    category: 'Booster',
    event: 'PURCHASE_COMPLETED',
    status: 'ACTIVE',
    validity: 'Fri-Sun only',
    priority: 200,
  },
  {
    id: 'rule_3',
    name: 'Signup Bonus',
    category: 'Event',
    event: 'MEMBER_REGISTERED',
    status: 'ACTIVE',
    validity: '01 Jan - 31 Mar',
    priority: 150,
  },
  {
    id: 'rule_4',
    name: 'Diwali Promo x3',
    category: 'Booster',
    event: 'PURCHASE_COMPLETED',
    status: 'PAUSED',
    validity: '01 Oct - 20 Oct',
    priority: 250,
  },
];

export default function EarningRulesPage() {
  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Earning Rules</h1>
        <div className="flex gap-2">
          <Link href="/loyalty/earning-rules/create">
            <Button>
              <IconPlus className="mr-2 h-4 w-4" />
              New Earning Rule
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
              <TableHead>Category</TableHead>
              <TableHead>Event</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Validity</TableHead>
              <TableHead>Priority</TableHead>
              <TableHead className="w-[50px]"></TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {earningRules.map((rule) => (
              <TableRow key={rule.id} className="hover:bg-muted/50">
                <TableCell className="font-medium">{rule.name}</TableCell>
                <TableCell><Badge variant="outline">{rule.category}</Badge></TableCell>
                <TableCell>{rule.event}</TableCell>
                <TableCell>
                  <Badge variant={rule.status === 'ACTIVE' ? 'success' : 'secondary'}>{rule.status}</Badge>
                </TableCell>
                <TableCell>{rule.validity}</TableCell>
                <TableCell>{rule.priority}</TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <span className="sr-only">Open menu</span>
                        <IconDotsVertical className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem>View Usage</DropdownMenuItem>
                      <DropdownMenuItem>Edit</DropdownMenuItem>
                      <DropdownMenuItem>Duplicate</DropdownMenuItem>
                      <DropdownMenuItem>{rule.status === 'ACTIVE' ? 'Pause' : 'Activate'}</DropdownMenuItem>
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
