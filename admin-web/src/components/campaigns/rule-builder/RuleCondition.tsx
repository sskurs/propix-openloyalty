"use client";

import { Trash2 } from "lucide-react";
import { RuleCondition as RuleConditionType, FIELD_DEFINITIONS, getOperatorsForFieldType, getOperatorLabel, FieldDefinition, Operator } from "@/types/rules";

interface RuleConditionProps {
    condition: RuleConditionType;
    onChange: (condition: RuleConditionType) => void;
    onDelete: () => void;
}

export function RuleCondition({ condition, onChange, onDelete }: RuleConditionProps) {
    const selectedField = FIELD_DEFINITIONS.find(f => f.name === condition.field);
    const availableOperators: Operator[] = selectedField
        ? getOperatorsForFieldType(selectedField.type)
        : ["eq"];

    const handleFieldChange = (fieldName: string) => {
        const field = FIELD_DEFINITIONS.find(f => f.name === fieldName);
        const operators: Operator[] = field ? getOperatorsForFieldType(field.type) : ["eq"];

        onChange({
            ...condition,
            field: fieldName,
            operator: operators[0], // Reset to first available operator
            value: null // Reset value when field changes
        });
    };

    const handleOperatorChange = (operator: string) => {
        onChange({
            ...condition,
            operator: operator as any
        });
    };

    const handleValueChange = (value: any) => {
        onChange({
            ...condition,
            value
        });
    };

    const renderValueInput = () => {
        if (!selectedField) {
            return (
                <input
                    type="text"
                    className="flex-1 px-3 py-2 bg-black/40 border border-amber-900/10 text-white rounded-md focus:outline-none focus:ring-1 focus:ring-amber-800/50 placeholder:text-amber-900"
                    placeholder="Select a field first"
                    disabled
                />
            );
        }

        // For fields with predefined options (like CustomerTier)
        if (selectedField.options) {
            if (condition.operator === "in" || condition.operator === "notIn") {
                // Multi-select for "in" operator
                return (
                    <select
                        multiple
                        className="flex-1 px-3 py-2 bg-amber-900/20 border border-amber-600/30 text-amber-50 rounded-md focus:outline-none focus:ring-2 focus:ring-amber-500 custom-scroll cursor-pointer"
                        value={Array.isArray(condition.value) ? condition.value : []}
                        onChange={(e) => {
                            const selected = Array.from(e.target.selectedOptions, option => option.value);
                            handleValueChange(selected);
                        }}
                    >
                        {selectedField.options.map(opt => (
                            <option key={opt.value} value={opt.value} className="bg-amber-950">{opt.label}</option>
                        ))}
                    </select>
                );
            } else {
                // Single select for other operators
                return (
                    <select
                        className="flex-1 px-3 py-2 bg-amber-900/20 border border-amber-600/30 text-amber-50 rounded-md focus:outline-none focus:ring-2 focus:ring-amber-500 cursor-pointer"
                        value={condition.value || ""}
                        onChange={(e) => handleValueChange(e.target.value)}
                    >
                        <option value="" className="bg-amber-950">Select value...</option>
                        {selectedField.options.map(opt => (
                            <option key={opt.value} value={opt.value} className="bg-amber-950">{opt.label}</option>
                        ))}
                    </select>
                );
            }
        }

        // For numeric fields
        if (selectedField.type === "number") {
            return (
                <input
                    type="number"
                    className="flex-1 px-3 py-2 bg-amber-900/20 border border-amber-600/30 text-amber-50 rounded-md focus:outline-none focus:ring-2 focus:ring-amber-500 placeholder:text-amber-700/50"
                    placeholder="Enter value"
                    value={condition.value || ""}
                    onChange={(e) => handleValueChange(parseFloat(e.target.value) || null)}
                />
            );
        }

        // For string fields
        return (
            <input
                type="text"
                className="flex-1 px-3 py-2 bg-[#120a00]/90 border border-amber-900/20 text-white rounded-md focus:outline-none focus:ring-1 focus:ring-amber-800/50 placeholder:text-amber-900 hover:border-amber-700 transition-colors"
                placeholder="Enter value"
                value={condition.value || ""}
                onChange={(e) => handleValueChange(e.target.value)}
            />
        );
    };

    return (
        <div className="flex items-center gap-2 p-3 bg-black/60 rounded-lg border border-amber-900/10 shadow-lg group">
            {/* Field Selector */}
            <select
                className="w-48 px-3 py-2 bg-[#120a00]/90 border border-amber-900/20 text-amber-50 rounded-md focus:outline-none focus:ring-1 focus:ring-amber-800/50 cursor-pointer hover:border-amber-700 transition-colors"
                value={condition.field}
                onChange={(e) => handleFieldChange(e.target.value)}
            >
                <option value="">Select field...</option>
                {Object.entries(
                    FIELD_DEFINITIONS.reduce((acc, field) => {
                        if (!acc[field.category]) acc[field.category] = [];
                        acc[field.category].push(field);
                        return acc;
                    }, {} as Record<string, FieldDefinition[]>)
                ).map(([category, fields]) => (
                    <optgroup key={category} label={category.charAt(0).toUpperCase() + category.slice(1)}>
                        {fields.map(field => (
                            <option key={field.name} value={field.name}>
                                {field.label}
                            </option>
                        ))}
                    </optgroup>
                ))}
            </select>

            {/* Operator Selector */}
            <select
                className="w-40 px-3 py-2 bg-[#120a00]/90 border border-amber-900/20 text-amber-50 rounded-md focus:outline-none focus:ring-1 focus:ring-amber-800/50 cursor-pointer disabled:opacity-30 hover:border-amber-700 transition-colors"
                value={condition.operator}
                onChange={(e) => handleOperatorChange(e.target.value)}
                disabled={!condition.field}
            >
                {availableOperators.map(op => (
                    <option key={op} value={op}>
                        {getOperatorLabel(op)}
                    </option>
                ))}
            </select>

            {/* Value Input */}
            {renderValueInput()}

            {/* Delete Button */}
            <button
                onClick={onDelete}
                className="p-2 text-red-400 hover:bg-red-400/10 rounded-md transition-colors"
                title="Delete condition"
            >
                <Trash2 className="w-4 h-4" />
            </button>
        </div>
    );
}
