export class NavigationItem {

  constructor(
    public title: string,
    public icon: string,
    public path?: string,
    public dynamicPath: () => string = () => path ? path : '',
    public visible?: () => boolean
  ) {

  }
}
