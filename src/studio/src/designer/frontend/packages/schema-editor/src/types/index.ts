/** Interfaces */
export interface ILanguage {
  [key: string]: string | ILanguage;
}

export interface ISchema {
  properties?: { [key: string]: { [key: string]: any } };
  definitions?: { [key: string]: { [key: string]: any } };
  $schema?: string;
  $id?: string;
  [key: string]: any;
}

export interface ISchemaState {
  schema: ISchema;
  uiSchema: UiSchemaItem[];
  name: string;
  saveSchemaUrl: string;
  selectedDefinitionNodeId: string;
  selectedPropertyNodeId: string;
  focusNameField?: string; // used to trigger focus of name field in inspector.
  navigate?: string; // used to trigger navigation in tree, the value is not used.
  selectedEditorTab: 'definitions' | 'properties';
}

export interface UiSchemaItem {
  path: string;
  type?: FieldType;
  $ref?: string;
  restrictions?: Restriction[];
  properties?: UiSchemaItem[];
  value?: any;
  displayName: string;
  required?: string[];
  title?: string;
  description?: string;
  items?: { type?: string; $ref?: string };
  enum?: string[];
  combination?: UiSchemaItem[];
  combinationKind?: CombinationKind;
  combinationItem?: boolean;
  isRequired?: boolean;
}

/** Types */
export enum CombinationKind {
  AllOf = 'allOf',
  AnyOf = 'anyOf',
  OneOf = 'oneOf',
}

/**
 * @link https://json-schema.org/understanding-json-schema/reference/type.html
 */
export enum FieldType {
  String = 'string',
  Integer = 'integer',
  Number = 'number',
  Boolean = 'boolean',
  Object = 'object',
  Array = 'array',
  Null = 'null',
}

export type NameInUseProps = {
  uiSchemaItems: UiSchemaItem[];
  parentSchema: UiSchemaItem | null;
  path: string;
  name: string;
};

export type Restriction = {
  key: string;
  value: any;
};
