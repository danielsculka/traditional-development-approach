export class Utils {
  static objectToFormData(obj: any, formData = new FormData(), parentKey = ''): FormData {
    for (const key in obj) {
      if (!obj.hasOwnProperty(key) || obj[key] === undefined) continue;

      const value = obj[key];
      const formKey = parentKey ? `${parentKey}.${key}` : key;

      if (value instanceof File) {
        formData.append(formKey, value);
      } else if (value === null) {
        formData.append(formKey, '');
      } else if (Array.isArray(value)) {
        value.forEach((item, index) => {
          if (item instanceof File) {
            formData.append(`${formKey}[${index}]`, item);
          } else {
            this.objectToFormData({ [index]: item }, formData, `${formKey}[${index}]`);
          }
        });
      } else if (typeof value === 'object' && !(value instanceof Date)) {
        this.objectToFormData(value, formData, formKey);
      } else {
        formData.append(formKey, value.toString());
      }
    }

    return formData;
  }
}
