import { FormControl } from "@angular/forms";

export function requiredFileType(type: string) {
  return function (control: FormControl) {
    const files: File[] | null = control.value;

    if (files) {
      const invalid = files.map(file => {
        const extension = file.name.split('.')[1].toLowerCase();

        return type.toLowerCase() !== extension.toLowerCase();
      }).filter(d => d);

      if (invalid.length) {
        return {
          requiredFileType: true
        };
      }
    }

    return null;
  };
}
