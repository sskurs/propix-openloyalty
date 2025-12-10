'use client';

import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue, SelectGroup, SelectLabel } from '@/components/ui/select';
import { Switch } from '@/components/ui/switch';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { useForm, useFieldArray, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { createEarningRule } from '@/api/earningRulesApi';
import { toast } from 'sonner';
import { useRouter } from 'next/navigation';

// --- Zod Schemas for Validation ---
const conditionSchema = z.object({
  field: z.string().min(1, "Field is required"),
  op: z.string().min(1, "Operator is required"),
  value: z.any().refine(val => val !== '' && val !== undefined, { message: "Value is required" }),
});

const formSchema = z.object({
  name: z.string().min(3, "Rule name is required"),
  description: z.string().optional(),
  active: z.boolean(),
  priority: z.coerce.number(),
  category: z.string().min(1, "Category is required"),
  eventKey: z.string().min(1, "Event is required"),
  condition: z.object({ operator: z.enum(['and', 'or']), rules: z.array(conditionSchema) }),
  points: z.object({ 
      type: z.string(), 
      points: z.coerce.number().optional(), 
      amount: z.coerce.number().optional(), 
      percentage: z.coerce.number().optional() 
  }),
  limits: z.object({ per_transaction: z.coerce.number().optional(), daily: z.coerce.number().optional() }).optional(),
  time_window: z.object({ start: z.string().optional(), end: z.string().optional() }).optional(),
  segments: z.array(z.string()).optional(),
}).refine(data => {
    if (data.points.type === 'per_amount') {
        return data.points.points != null && data.points.amount != null;
    }
    if (data.points.type === 'fixed') {
        return data.points.points != null;
    }
    if (data.points.type === 'multiplier') {
        return data.points.percentage != null;
    }
    return true;
}, { message: "Please provide required values for the selected formula.", path: ["points"] });

type FormValues = z.infer<typeof formSchema>;

const conditionFields = {
  "Event Payload": {
    "event.payload.gross_value": "Gross Value",
    "event.payload.category": "Category",
  },
  "User Snapshot": { "user.Tier": "Tier" }
};

const ConditionBuilder = ({ control, errors }: { control: any; errors: any }) => {
  const { fields, append, remove } = useFieldArray({ control, name: "condition.rules" });
  return (
    <div className="p-4 border rounded-lg bg-slate-50 dark:bg-slate-900/50">
      <div className="flex items-center space-x-2"><Controller control={control} name="condition.operator" render={({ field }) => (<Select onValueChange={field.onChange} defaultValue={field.value}><SelectTrigger className="w-auto"><SelectValue /></SelectTrigger><SelectContent><SelectItem value="and">Match ALL (AND)</SelectItem><SelectItem value="or">Match ANY (OR)</SelectItem></SelectContent></Select>)} /><p>of the following conditions:</p></div>
      <div className="space-y-3 mt-4 pl-6 border-l-2 border-slate-300 dark:border-slate-700">
        {fields.map((item, index) => (
          <div key={item.id} className="flex items-start space-x-2">
            <div className="w-48 space-y-1"><Controller name={`condition.rules.${index}.field`} control={control} render={({ field }) => (<Select onValueChange={field.onChange} defaultValue={field.value}><SelectTrigger><SelectValue placeholder="Field..." /></SelectTrigger><SelectContent>{Object.entries(conditionFields).map(([group, options]) => (<SelectGroup key={group}><SelectLabel>{group}</SelectLabel>{Object.entries(options).map(([value, label]) => (<SelectItem key={value} value={value}>{label}</SelectItem>))}</SelectGroup>))}</SelectContent></Select>)} />{errors.condition?.rules?.[index]?.field && <p className="text-xs text-red-500">{errors.condition.rules[index].field.message}</p>}</div>
            <div className="w-32 space-y-1"><Controller name={`condition.rules.${index}.op`} control={control} render={({ field }) => (<Select onValueChange={field.onChange} defaultValue={field.value}><SelectTrigger><SelectValue placeholder="Operator..." /></SelectTrigger><SelectContent><SelectItem value="eq">eq</SelectItem><SelectItem value="gte">gte</SelectItem></SelectContent></Select>)} />{errors.condition?.rules?.[index]?.op && <p className="text-xs text-red-500">{errors.condition.rules[index].op.message}</p>}</div>
            <div className="w-48 space-y-1"><Controller name={`condition.rules.${index}.value`} control={control} render={({ field }) => <Input {...field} placeholder="Value..." />} />{errors.condition?.rules?.[index]?.value && <p className="text-xs text-red-500">{errors.condition.rules[index].value.message}</p>}</div>
            <Button type="button" variant="ghost" size="icon" className="mt-2" onClick={() => remove(index)}><IconTrash className="h-4 w-4 text-muted-foreground" /></Button>
          </div>
        ))}
        <div className="pt-2"><Button type="button" variant="outline" size="sm" onClick={() => append({ field: '', op: '', value: '' })}><IconPlus className="h-4 w-4 mr-2" />Add Condition</Button></div>
      </div>
    </div>
  );
};

const PointsBuilder = ({ control, watch, errors }: { control: any, watch: any, errors: any }) => {
    const type = watch("points.type");
    return (
        <div className="space-y-4">
            <div className="space-y-2">
                <Label>Formula Type</Label>
                <Controller control={control} name="points.type" render={({ field }) => (<Select onValueChange={field.onChange} defaultValue={field.value}><SelectTrigger><SelectValue/></SelectTrigger><SelectContent><SelectItem value="fixed">Fixed</SelectItem><SelectItem value="per_amount">Per Amount</SelectItem><SelectItem value="multiplier">Multiplier</SelectItem></SelectContent></Select>)} />
            </div>

            <div className="space-y-2">
                {type === 'fixed' && <div className="flex items-center gap-2"><Label className="font-mono text-sm">points:</Label><Controller name="points.points" control={control} render={({ field }) => <Input type="number" {...field} />} /></div>}
                {type === 'per_amount' && <div className="flex items-center gap-2"><Label className="font-mono text-sm">amount:</Label><Controller name="points.amount" control={control} render={({ field }) => <Input type="number" {...field} />} /><Label className="font-mono text-sm">points:</Label><Controller name="points.points" control={control} render={({ field }) => <Input type="number" {...field} />} /></div>}
                {type === 'multiplier' && <div className="flex items-center gap-2"><Label className="font-mono text-sm">percentage:</Label><Controller name="points.percentage" control={control} render={({ field }) => <Input type="number" {...field} />} /></div>}
                {errors.points && <p className="text-sm text-red-500 mt-2">{errors.points.message}</p>}
            </div>
        </div>
    );
}

export default function CreateEarningRulePage() {
  const router = useRouter();
  const { register, handleSubmit, control, watch, formState: { errors } } = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "", description: "", active: true, priority: 100, category: 'BASE', eventKey: 'PURCHASE_COMPLETED', condition: { operator: 'and', rules: [] }, points: { type: 'fixed' },
    },
  });

  const onSubmit = async (values: FormValues) => {
    const payload = { ...values, status: values.active ? 'ACTIVE' : 'DRAFT' };
    const promise = createEarningRule(payload as any);
    toast.promise(promise, { loading: 'Creating earning rule...', success: (data) => { router.push('/loyalty/earning-rules'); return `Rule "${data.name}" has been created.`; }, error: (err) => err.message || 'An unexpected error occurred.' });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-8">
      <div className="flex justify-between items-center"><div><h1 className="text-3xl font-bold">Create Earning Rule</h1><p className="text-muted-foreground">Define when and how many points members will earn.</p></div><div className="flex gap-2"><Button type="button" variant="outline" onClick={() => router.back()}>Cancel</Button><Button type="submit">Save & Activate</Button></div></div>
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 items-start">
        <div className="lg:col-span-2 space-y-6">
           <Card>
            <CardHeader><CardTitle>Rule Details</CardTitle></CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2"><Label>Rule Name</Label><Input placeholder="e.g., High-Value Electronics Booster" {...register("name")} />{errors.name && <p className="text-sm text-red-500 mt-1">{errors.name.message}</p>}</div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2"><Label>Category</Label><Controller control={control} name="category" render={({ field }) => (<Select onValueChange={field.onChange} defaultValue={field.value}><SelectTrigger><SelectValue /></SelectTrigger><SelectContent><SelectItem value="BASE">Base</SelectItem><SelectItem value="BOOSTER">Booster</SelectItem><SelectItem value="EVENT">Event</SelectItem></SelectContent></Select>)} /></div>
                <div className="space-y-2"><Label>Trigger Event</Label><Controller control={control} name="eventKey" render={({ field }) => (<Select onValueChange={field.onChange} defaultValue={field.value}><SelectTrigger><SelectValue /></SelectTrigger><SelectContent><SelectItem value="PURCHASE_COMPLETED">Purchase Completed</SelectItem><SelectItem value="MEMBER_REGISTERED">Member Registered</SelectItem></SelectContent></Select>)} /></div>
              </div>
              <div className="space-y-2"><Label>Description</Label><Textarea placeholder="Extra points for electronics purchases" {...register("description")} /></div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader><CardTitle>Conditions</CardTitle></CardHeader>
            <CardContent><ConditionBuilder control={control} errors={errors} /></CardContent>
          </Card>
          <Card>
            <CardHeader><CardTitle>Points Formula</CardTitle></CardHeader>
            <CardContent><PointsBuilder control={control} watch={watch} errors={errors} /></CardContent>
          </Card>
        </div>
        <div className="lg:col-span-1 space-y-6">
          <Card>
              <CardHeader><CardTitle>Configuration</CardTitle></CardHeader>
              <CardContent className="space-y-4"><Controller control={control} name="active" render={({ field }) => (<div className="flex items-center justify-between rounded-lg border p-3"><Label>Rule Active</Label><Switch checked={field.value} onCheckedChange={field.onChange} /></div>)} /><div className="space-y-2"><Label>Priority</Label><Input type="number" placeholder="100" {...register("priority")} />{errors.priority && <p className="text-sm text-red-500 mt-1">{errors.priority.message}</p>}</div></CardContent>
          </Card>
          <Card>
              <CardHeader><CardTitle>Time Window</CardTitle></CardHeader>
              <CardContent className="space-y-4"><div className="space-y-2"><Label>Start Date</Label><Input type="datetime-local" {...register("time_window.start")} /></div><div className="space-y-2"><Label>End Date</Label><Input type="datetime-local" {...register("time_window.end")} /></div></CardContent>
          </Card>
          <Card>
              <CardHeader><CardTitle>Limits</CardTitle></CardHeader>
              <CardContent className="space-y-4"><div className="space-y-2"><Label>Per Transaction</Label><Input type="number" {...register("limits.per_transaction")} /></div><div className="space-y-2"><Label>Daily</Label><Input type="number" {...register("limits.daily")} /></div></CardContent>
          </Card>
     
        </div>
      </div>
    </form>
  );
}
