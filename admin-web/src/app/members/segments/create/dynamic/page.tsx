'use client';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue, SelectGroup, SelectLabel } from '@/components/ui/select';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { useForm, useFieldArray, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { createSegment } from '@/api/segmentsApi';
import { toast } from 'sonner';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import { Badge } from '@/components/ui/badge';

const conditionSchema = z.object({
  field: z.string().min(1, "Field is required"),
  operator: z.string().min(1, "Operator is required"),
  value: z.string().min(1, "Value is required"),
  period_days: z.number().optional(),
});

const formSchema = z.object({
  name: z.string().min(3, 'Segment name must be at least 3 characters'),
  description: z.string().optional(),
  rule: z.object({
    and: z.array(conditionSchema).optional(),
    or: z.array(conditionSchema).optional(),
  }),
});

type FormValues = z.infer<typeof formSchema>;

const ruleFields = {
    RFM: ['recency_days', 'frequency', 'monetary'],
    Profile: ['age', 'gender', 'country', 'city'],
    Loyalty: ['tier', 'points_balance'],
};

const RuleRow = ({ control, index, onRemove, field: formField }: { control: any, index: number, onRemove: (index: number) => void, field: any }) => {
    const [selectedField, setSelectedField] = useState(formField.field);
    const showPeriodInput = ['frequency', 'monetary'].includes(selectedField);

    return (
        <div className="flex items-center space-x-2 p-3 rounded-lg bg-white dark:bg-slate-900 border border-slate-200 dark:border-slate-700">
            <Controller
                control={control}
                name={`rule.and[${index}].field`}
                render={({ field }) => (
                    <Select onValueChange={(value) => { field.onChange(value); setSelectedField(value); }} defaultValue={field.value}>
                        <SelectTrigger className="w-48"><SelectValue placeholder="Select a field..." /></SelectTrigger>
                        <SelectContent>
                            {Object.entries(ruleFields).map(([group, options]) => (
                                <div key={group}>
                                    <Label className="px-2 py-1.5 text-xs font-semibold text-muted-foreground">{group}</Label>
                                    {options.map(opt => <SelectItem value={opt} key={opt}>{opt.replace('_', ' ')}</SelectItem>)}
                                </div>
                            ))}
                        </SelectContent>
                    </Select>
                )}
            />

            <Controller
                control={control}
                name={`rule.and[${index}].operator`}
                render={({ field }) => (
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <SelectTrigger className="w-32"><SelectValue placeholder="Operator" /></SelectTrigger>
                        <SelectContent>
                            <SelectItem value="gte">&gt;= (is at least)</SelectItem>
                            <SelectItem value="lte">&lt;= (is at most)</SelectItem>
                            <SelectItem value="eq">= (is equal to)</SelectItem>
                            <SelectItem value="neq">!= (is not)</SelectItem>
                        </SelectContent>
                    </Select>
                )}
            />

            <Controller
                control={control}
                name={`rule.and[${index}].value`}
                render={({ field }) => <Input {...field} placeholder="Value" className="w-32" />}
            />

            {showPeriodInput && (
                <div className="flex items-center gap-2 whitespace-nowrap text-sm">
                    <span>in the last</span>
                     <Controller
                        control={control}
                        name={`rule.and[${index}].period_days`}
                        render={({ field }) => <Input {...field} placeholder="e.g., 365" className="w-24" type="number" onChange={e => field.onChange(parseInt(e.target.value, 10) || null)} />}
                    />
                    <span>days</span>
                </div>
            )}

            <Button variant="ghost" size="icon" type="button" onClick={() => onRemove(index)}>
                <IconTrash className="h-4 w-4 text-red-500" />
            </Button>
        </div>
    );
};

const RuleBuilder = ({ control, register, errors }: { control: any; register: any; errors: any; }) => {
    const { fields, append, remove } = useFieldArray({ control, name: "rule.and" });

    return (
        <div className="p-4 border rounded-md bg-slate-50 dark:bg-slate-800/50 mt-6">
            <div className="flex items-center space-x-2 mb-4">
                <Label className="font-semibold text-lg">Conditions</Label>
                <Controller
                    control={control}
                    name="conditions.logic"
                    render={({ field }) => (
                        <Select onValueChange={field.onChange} defaultValue="and">
                            <SelectTrigger className="w-auto"><SelectValue /></SelectTrigger>
                            <SelectContent>
                                <SelectItem value="and">Match ALL (AND)</SelectItem>
                                <SelectItem value="or">Match ANY (OR)</SelectItem>
                            </SelectContent>
                        </Select>
                    )}
                />
            </div>

            <div className="space-y-3 pl-4">
                {fields.map((field, index) => (
                    <RuleRow key={field.id} control={control} index={index} onRemove={remove} field={field} />
                ))}
                 <div className="pt-2">
                    <Button variant="outline" size="sm" type="button" onClick={() => append({ field: '', operator: '', value: '' })}>
                        <IconPlus className="h-4 w-4 mr-2" />Add Condition
                    </Button>
                    {errors.rule?.and?.root && <p className="text-sm text-red-500 mt-2">{errors.rule.and.root.message}</p>}
                 </div>
            </div>
        </div>
    )
}

export default function CreateDynamicSegmentPage() {
  const router = useRouter();
  const { register, handleSubmit, control, formState: { errors } } = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: 'Champions',
      description: 'Recency ≤ 30 & Frequency ≥ 4 & Monetary ≥ 1000',
      rule: {
        and: [
          { field: 'recency_days', operator: 'lte', value: '30' },
          { field: 'frequency', operator: 'gte', value: '4', period_days: 365 },
          { field: 'monetary', operator: 'gte', value: '1000', period_days: 365 },
        ],
      },
    },
  });

  const onSubmit = async (values: FormValues) => {
    const formattedRule: any = { and: [] };
    values.rule.and?.forEach(rule => {
        const newRule: any = {};
        const key = rule.field; 
        newRule[key] = { [rule.operator]: parseInt(rule.value, 10) };
        if (rule.period_days) {
            newRule[key].period_days = rule.period_days;
        }
        formattedRule.and.push(newRule);
    });
    
    const payload = {
        name: values.name,
        description: values.description,
        kind: 'dynamic',
        definition: JSON.stringify({ name: values.name, kind: 'dynamic', description: values.description, rule: formattedRule })
    };

    const promise = createSegment(payload as any);

    toast.promise(promise, {
      loading: 'Creating segment...',
      success: () => {
        router.push('/members/segments');
        return `Segment "${values.name}" has been created successfully!`
      },
      error: (err) => err.message || 'An unexpected error occurred.'
    });
  };

  return (
    <div>
      <h1 className="text-3xl font-bold mb-4">Create Dynamic Segment</h1>
      <p className="text-muted-foreground mb-8">Define rules based on member behavior and attributes to create a dynamic segment.</p>
      <form onSubmit={handleSubmit(onSubmit)}>
        <Card className="max-w-5xl mx-auto">
          <CardHeader>
            <CardTitle>Segment Details</CardTitle>
          </CardHeader>
          <CardContent>
              <div className="space-y-4">
                  <div className="space-y-2">
                      <Label htmlFor="name">Segment Name</Label>
                      <Input id="name" placeholder="e.g., Champions" {...register('name')} />
                  </div>
                  <div className="space-y-2">
                      <Label htmlFor="description">Description</Label>
                      <Textarea id="description" placeholder="Members with high recency, frequency, and monetary value." {...register('description')} />
                  </div>
                  <RuleBuilder control={control} register={register} errors={errors} />
              </div>
          </CardContent>
          <CardFooter className="flex justify-end gap-2">
              <Button type="button" variant="secondary" onClick={() => router.back()}>Cancel</Button>
              <Button type="submit">Create & Activate</Button>
          </CardFooter>
        </Card>
      </form>
    </div>
  );
}
