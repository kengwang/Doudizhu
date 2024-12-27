/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export type DoudizhuApiModelsGameLogicGame = DoudizhuApiModelsGuidModelBase & {
  /** @format date-time */
  createAt?: string;
  users?: DoudizhuApiModelsGameLogicGameUser[];
  reservedCards?: DoudizhuApiModelsGameLogicCard[];
  records?: DoudizhuApiModelsGameLogicGameRecord[];
  lastCardSentence?: DoudizhuApiModelsGameLogicCardSentence | null;
  lastUser?: DoudizhuApiModelsGameLogicGameUser | null;
  status?: DoudizhuApiModelsGameLogicGameStatus;
  currentUser?: DoudizhuApiModelsGameLogicGameUser | null;
  /** @format guid */
  currentUserId?: string;
  /** @format guid */
  lastUserId?: string;
};

export type DoudizhuApiModelsGameLogicGameUser = DoudizhuApiModelsGuidModelBase & {
  /** @format guid */
  gameId?: string;
  user?: DoudizhuApiModelsUser;
  /** @format int32 */
  calledLandLordCount?: number;
  cards?: DoudizhuApiModelsGameLogicCard[];
  role?: DoudizhuApiModelsGameLogicGameUserRole;
  botTakeOver?: boolean;
};

export type DoudizhuApiModelsUser = DoudizhuApiModelsGuidModelBase & {
  /** @maxLength 60 */
  name?: string;
  /** @format int64 */
  coin?: number;
  /** @maxLength 20 */
  qq?: string | null;
};

export interface DoudizhuApiModelsGuidModelBase {
  /** @format guid */
  id?: string;
}

export interface DoudizhuApiModelsGameLogicCard {
  color?: DoudizhuApiModelsGameLogicCardColor;
  number?: DoudizhuApiModelsGameLogicCardNumber;
}

export enum DoudizhuApiModelsGameLogicCardColor {
  Meihua = 0,
  Fangkuai = 1,
  Hongtao = 2,
  Heitao = 3,
  Special = 4,
}

export enum DoudizhuApiModelsGameLogicCardNumber {
  Three = 0,
  Four = 1,
  Five = 2,
  Six = 3,
  Seven = 4,
  Eight = 5,
  Nine = 6,
  Ten = 7,
  J = 8,
  Q = 9,
  K = 10,
  A = 11,
  Two = 12,
  SmallJoker = 13,
  BigJoker = 14,
}

export enum DoudizhuApiModelsGameLogicGameUserRole {
  Undefined = 0,
  Landlord = 1,
  Farmer = 2,
}

export type DoudizhuApiModelsGameLogicGameRecord = DoudizhuApiModelsGuidModelBase & {
  game?: DoudizhuApiModelsGameLogicGame;
  gameUser?: DoudizhuApiModelsGameLogicGameUser;
  cardSentence?: DoudizhuApiModelsGameLogicCardSentence | null;
};

export interface DoudizhuApiModelsGameLogicCardSentence {
  pattern?: DoudizhuApiModelsGameLogicCardPatternType;
  cards?: DoudizhuApiModelsGameLogicCard[];
}

export enum DoudizhuApiModelsGameLogicCardPatternType {
  Single = 0,
  Pair = 1,
  MultiPair = 2,
  ThreeWithOne = 3,
  ThreeWithPair = 4,
  FourWithPair = 5,
  Straight = 6,
  Triple = 7,
  PlaneWithPair = 8,
  Bomb = 9,
  JokerBomb = 10,
}

export enum DoudizhuApiModelsGameLogicGameStatus {
  Waiting = 0,
  Starting = 1,
  Running = 2,
  Ended = 3,
}

export interface DoudizhuApiEndpointsGameRecordsPlayCardRequest {
  cards?: DoudizhuApiModelsGameLogicCard[];
}

export interface DoudizhuApiEndpointsUserUserLoginRequest {
  qq?: string;
}

export interface DoudizhuApiEndpointsUserUserRegisterRequest {
  userName?: string;
  qq?: string;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseFormat;
  /** request body */
  body?: unknown;
  /** base url */
  baseUrl?: string;
  /** request cancellation token */
  cancelToken?: CancelToken;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> {
  baseUrl?: string;
  baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
  securityWorker?: (securityData: SecurityDataType | null) => Promise<RequestParams | void> | RequestParams | void;
  customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown> extends Response {
  data: D;
  error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public baseUrl: string = "http://localhost:44460";
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private abortControllers = new Map<CancelToken, AbortController>();
  private customFetch = (...fetchParams: Parameters<typeof fetch>) => fetch(...fetchParams);

  private baseApiParams: RequestParams = {
    credentials: "same-origin",
    headers: {},
    redirect: "follow",
    referrerPolicy: "no-referrer",
  };

  constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
    Object.assign(this, apiConfig);
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected encodeQueryParam(key: string, value: any) {
    const encodedKey = encodeURIComponent(key);
    return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
  }

  protected addQueryParam(query: QueryParamsType, key: string) {
    return this.encodeQueryParam(key, query[key]);
  }

  protected addArrayQueryParam(query: QueryParamsType, key: string) {
    const value = query[key];
    return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
  }

  protected toQueryString(rawQuery?: QueryParamsType): string {
    const query = rawQuery || {};
    const keys = Object.keys(query).filter((key) => "undefined" !== typeof query[key]);
    return keys
      .map((key) => (Array.isArray(query[key]) ? this.addArrayQueryParam(query, key) : this.addQueryParam(query, key)))
      .join("&");
  }

  protected addQueryParams(rawQuery?: QueryParamsType): string {
    const queryString = this.toQueryString(rawQuery);
    return queryString ? `?${queryString}` : "";
  }

  private contentFormatters: Record<ContentType, (input: any) => any> = {
    [ContentType.Json]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string") ? JSON.stringify(input) : input,
    [ContentType.Text]: (input: any) => (input !== null && typeof input !== "string" ? JSON.stringify(input) : input),
    [ContentType.FormData]: (input: any) =>
      Object.keys(input || {}).reduce((formData, key) => {
        const property = input[key];
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
              ? JSON.stringify(property)
              : `${property}`,
        );
        return formData;
      }, new FormData()),
    [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
  };

  protected mergeRequestParams(params1: RequestParams, params2?: RequestParams): RequestParams {
    return {
      ...this.baseApiParams,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...(this.baseApiParams.headers || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected createAbortSignal = (cancelToken: CancelToken): AbortSignal | undefined => {
    if (this.abortControllers.has(cancelToken)) {
      const abortController = this.abortControllers.get(cancelToken);
      if (abortController) {
        return abortController.signal;
      }
      return void 0;
    }

    const abortController = new AbortController();
    this.abortControllers.set(cancelToken, abortController);
    return abortController.signal;
  };

  public abortRequest = (cancelToken: CancelToken) => {
    const abortController = this.abortControllers.get(cancelToken);

    if (abortController) {
      abortController.abort();
      this.abortControllers.delete(cancelToken);
    }
  };

  public request = async <T = any, E = any>({
    body,
    secure,
    path,
    type,
    query,
    format,
    baseUrl,
    cancelToken,
    ...params
  }: FullRequestParams): Promise<HttpResponse<T, E>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const queryString = query && this.toQueryString(query);
    const payloadFormatter = this.contentFormatters[type || ContentType.Json];
    const responseFormat = format || requestParams.format;

    return this.customFetch(`${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`, {
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type && type !== ContentType.FormData ? { "Content-Type": type } : {}),
      },
      signal: (cancelToken ? this.createAbortSignal(cancelToken) : requestParams.signal) || null,
      body: typeof body === "undefined" || body === null ? null : payloadFormatter(body),
    }).then(async (response) => {
      const r = response.clone() as HttpResponse<T, E>;
      r.data = null as unknown as T;
      r.error = null as unknown as E;

      const data = !responseFormat
        ? r
        : await response[responseFormat]()
            .then((data) => {
              if (r.ok) {
                r.data = data;
              } else {
                r.error = data;
              }
              return r;
            })
            .catch((e) => {
              r.error = e;
              return r;
            });

      if (cancelToken) {
        this.abortControllers.delete(cancelToken);
      }

      if (!response.ok) throw data;
      return data;
    });
  };
}

/**
 * @title Doudizhu.Api
 * @version 1.0.0
 * @baseUrl http://localhost:44460
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  games = {
    /**
     * No description
     *
     * @tags Games
     * @name CreateGameEndpoint
     * @request POST:/api/games
     * @secure
     */
    createGameEndpoint: (params: RequestParams = {}) =>
      this.request<string, string>({
        path: `/api/games`,
        method: "POST",
        secure: true,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Games
     * @name GetGameEndpoint
     * @request GET:/api/games/{id}
     * @secure
     */
    getGameEndpoint: (id: string, params: RequestParams = {}) =>
      this.request<DoudizhuApiModelsGameLogicGame, string>({
        path: `/api/games/${id}`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Games
     * @name GetGamesEndpoint
     * @request GET:/api/games
     * @secure
     */
    getGamesEndpoint: (params: RequestParams = {}) =>
      this.request<DoudizhuApiModelsGameLogicGame[], string>({
        path: `/api/games`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Games
     * @name JoinGameEndpoint
     * @request POST:/api/games/{id}/gameUser
     * @secure
     */
    joinGameEndpoint: (id: string, params: RequestParams = {}) =>
      this.request<DoudizhuApiModelsGameLogicGameUser, string>({
        path: `/api/games/${id}/gameUser`,
        method: "POST",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Games
     * @name PlayCardEndpoint
     * @request POST:/api/games/{id}/records
     * @secure
     */
    playCardEndpoint: (id: string, data: DoudizhuApiEndpointsGameRecordsPlayCardRequest, params: RequestParams = {}) =>
      this.request<string, string>({
        path: `/api/games/${id}/records`,
        method: "POST",
        body: data,
        secure: true,
        type: ContentType.Json,
        ...params,
      }),
  };
  user = {
    /**
     * No description
     *
     * @tags User
     * @name GetUserStatusEndpoint
     * @request GET:/api/user/status
     * @secure
     */
    getUserStatusEndpoint: (params: RequestParams = {}) =>
      this.request<DoudizhuApiModelsUser, string>({
        path: `/api/user/status`,
        method: "GET",
        secure: true,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name UserLoginEndpoint
     * @request POST:/api/user/login
     */
    userLoginEndpoint: (data: DoudizhuApiEndpointsUserUserLoginRequest, params: RequestParams = {}) =>
      this.request<DoudizhuApiModelsUser, string>({
        path: `/api/user/login`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name UserRegisterEndpoint
     * @request POST:/api/user/register
     */
    userRegisterEndpoint: (data: DoudizhuApiEndpointsUserUserRegisterRequest, params: RequestParams = {}) =>
      this.request<string, any>({
        path: `/api/user/register`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),
  };
}
