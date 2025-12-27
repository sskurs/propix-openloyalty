'use client';

import { useEffect, useState } from 'react';
import { getEarningRules, EarningRule } from '@/api/earningRulesApi';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger, DropdownMenuSeparator } from "@/components/ui/dropdown-menu";
import { IconDotsVertical, IconFilter, IconPlus, IconSearch } from "@tabler/icons-react";
import { Input } from "@/components/ui/input";
import Link from "next/link";
import { format } from 'date-fns';

export default function EarningRulesPage() {
  const [rules, setRules] = useState<EarningRule[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getEarningRules().then(data => {
        setRules(data || []);
        setLoading(false);
    });
  }, []);

  const renderValidity = (rule: EarningRule) => {
      if (rule.activateAt && rule.deactivateAt) {
          return `${format(new Date(rule.activateAt), 'dd MMM')} - ${format(new Date(rule.deactivateAt), 'dd MMM yyyy')}`;
      }
      if (rule.activateAt) {
          return `Starts ${format(new Date(rule.activateAt), 'dd MMM yyyy')}`;
      }
      if (rule.cronExpression) {
        return rule.cronExpression;
      }
      return 'Always Active';
  }

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
             {loading ? (
                <TableRow><TableCell colSpan={7} className="text-center">Loading...</TableCell></TableRow>
             ) : (
                rules.map((rule) => (
                <TableRow key={rule.id} className="hover:bg-muted/50">
                    <TableCell className="font-medium">{rule.name}</TableCell>
                    <TableCell><Badge variant="outline">{rule.category}</Badge></TableCell>
                    <TableCell>{rule.eventKey}</TableCell>
                    <TableCell>
                    <Badge variant={rule.status === 'ACTIVE' ? 'success' : 'secondary'}>{rule.status}</Badge>
                    </TableCell>
                    <TableCell>{renderValidity(rule)}</TableCell>
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
                ))
             )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
