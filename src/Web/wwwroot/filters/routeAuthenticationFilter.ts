export class RouteAuthenticationFilterValueConverter {
  toView(routes: any, isLoggedIn: boolean){
    return routes.filter(route => {
      var requiresAuthentication = route.config.authenticated;
      
      if (requiresAuthentication === undefined)
        return true;
      
      return (isLoggedIn === requiresAuthentication);
    });
  }
}