type RouteHandler = (params: Record<string, string>) => Promise<void> | void;

interface Route {
  pattern: RegExp;
  paramNames: string[];
  handler: RouteHandler;
}

const routes: Route[] = [];

export const router = {
  on(path: string, handler: RouteHandler): void {
    const paramNames: string[] = [];
    const regexStr = path
      .replace(/:([^/]+)/g, (_, name) => {
        paramNames.push(name);
        return '([^/]+)';
      })
      .replace(/\//g, '\\/');
    routes.push({ pattern: new RegExp(`^${regexStr}$`), paramNames, handler });
  },

  navigate(hash: string): void {
    const path = hash.replace(/^#/, '') || '/';
    for (const route of routes) {
      const match = path.match(route.pattern);
      if (match) {
        const params: Record<string, string> = {};
        route.paramNames.forEach((name, i) => {
          params[name] = match[i + 1];
        });
        route.handler(params);
        return;
      }
    }
  },

  start(): void {
    window.addEventListener('hashchange', () => router.navigate(window.location.hash));
    router.navigate(window.location.hash);
  },
};
