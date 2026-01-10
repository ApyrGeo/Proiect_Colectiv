import type { OptionalPackageProps } from "./props";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { useCallback } from "react";

export type OptionalsCallback = (data: { semester1or2: number, packageId: number, subjectId: number }) => void

export interface OptionalsState {
    optionals: OptionalPackageProps[]
    updateOptional: OptionalsCallback
}

export function ContractOptionals(state: OptionalsState) {
    const perSemester = new Map<Number, OptionalPackageProps[]>()
    for (const optional of state.optionals)
    {
        const collection = perSemester.get(optional.semesterNumber)
        if (!collection)
            perSemester.set(optional.semesterNumber, [optional])
        else
            collection.push(optional)
    }

    const handleChange = useCallback((semester1or2: number, packageId: number, subjectId: number | '') => {
        if (!subjectId)
            return
        state.updateOptional({ semester1or2, packageId, subjectId })
    }, [])

    return (
        <div>
            {[...perSemester.entries()].map(([semesterNumber, optionals]) => 
                <div key={`optional-${semesterNumber}`}>
                    <p>Semester {semesterNumber.valueOf()}</p>
                    {optionals.map(optional =>
                        <FormControl key={`optional-id-${optional.packageId}`} fullWidth>
                            <InputLabel id={`contracts-optional-${semesterNumber}-${optional.packageId}`}>
                                {labelOfPkg(optional.packageId)}
                            </InputLabel>

                            <Select<number>
                                label={labelOfPkg(optional.packageId)}
                                onChange={x => handleChange(optional.semester1or2, optional.packageId, x.target.value)}
                            >
                                {optional.subjects.map(x =>
                                    <MenuItem value={x.id} key={x.id}>{x.name}</MenuItem>
                                )}
                            </Select>
                        </FormControl>
                    )}
                </div>
            )}
        </div>
            
    )
}``

function labelOfPkg(pkg: number) {
    return `Package ${pkg} of optionals`
}

/* map(([semesterNumber, optionals]) => {
                <div key={`optional-${semesterNumber}`}>
                    <p>Semester {semesterNumber.valueOf()}</p>
                    {optionals.map(optional =>
                        <FormControl key={`optional-id-${optional.packageId}`} fullWidth>
                            <Select<number>
                                label={`Package ${optional.packageId} of optionals`}
                                onChange={x => handleChange(optional.semester1or2, optional.packageId, x.target.value )}
                            >
                                {optional.subjects.map(x =>
                                    <MenuItem value={x.id} key={x.id}>{x.name}</MenuItem>
                                )}
                            </Select>
                        </FormControl>  
                    )}
                </div>
                })} */