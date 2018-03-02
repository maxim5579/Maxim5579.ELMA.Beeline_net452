<?xml version="1.0" encoding="utf-8"?>
<Entity xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Uid>a20e56f1-5ec9-4aa2-a926-b4e0972d980b</Uid>
  <Name>CallRecord</Name>
  <DisplayName>CallRecord</DisplayName>
  <Images />
  <NamedImages />
  <Namespace>Maxim5579.ELMA.Beeline_net452.Models</Namespace>
  <BaseClassUid>00000000-0000-0000-0000-000000000000</BaseClassUid>
  <Properties>
    <PropertyMetadata xsi:type="EntityPropertyMetadata">
      <Uid>2d38aff5-bee4-4364-94d4-069571b39f09</Uid>
      <Name>Uid</Name>
      <DisplayName>Уникальный идентификатор</DisplayName>
      <Images />
      <NamedImages />
      <TypeUid>eb6e8ddc-fafe-4e0e-9018-1a7667012579</TypeUid>
      <SubTypeUid>00000000-0000-0000-0000-000000000000</SubTypeUid>
      <Settings xsi:type="GuidSettings">
        <FieldName>Uid</FieldName>
      </Settings>
      <Nullable>false</Nullable>
      <IsSystem>true</IsSystem>
      <OwnerPropertyUid>00000000-0000-0000-0000-000000000000</OwnerPropertyUid>
      <ViewSettings>
        <Attributes>
          <ViewAttribute>
            <Visibility>Hidden</Visibility>
            <ReadOnly>true</ReadOnly>
          </ViewAttribute>
        </Attributes>
      </ViewSettings>
      <Order>0</Order>
      <Handlers />
      <TypeResolver>00000000-0000-0000-0000-000000000000</TypeResolver>
    </PropertyMetadata>
    <PropertyMetadata xsi:type="EntityPropertyMetadata">
      <Uid>caf7eacb-91de-4074-9d1c-4a35577488cc</Uid>
      <Name>Poljzovatelj</Name>
      <DisplayName>Пользователь</DisplayName>
      <Images />
      <NamedImages />
      <TypeUid>72ed98ca-f260-4671-9bcd-ff1d80235f47</TypeUid>
      <SubTypeUid>cfdeb03c-35e9-45e7-aad8-037d83888f73</SubTypeUid>
      <Settings xsi:type="EntityUserSettings">
        <FieldName>Poljzovatelj</FieldName>
        <CascadeMode>SaveUpdate</CascadeMode>
      </Settings>
      <Nullable>true</Nullable>
      <OwnerPropertyUid>00000000-0000-0000-0000-000000000000</OwnerPropertyUid>
      <ViewSettings>
        <Attributes>
          <ViewAttribute>
            <ViewType>Create</ViewType>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Edit</ViewType>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Display</ViewType>
            <ReadOnly>true</ReadOnly>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>List</ViewType>
            <Visibility>Hidden</Visibility>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Filter</ViewType>
          </ViewAttribute>
        </Attributes>
      </ViewSettings>
      <Order>1</Order>
      <Handlers />
      <TypeResolver>00000000-0000-0000-0000-000000000000</TypeResolver>
    </PropertyMetadata>
    <PropertyMetadata xsi:type="EntityPropertyMetadata">
      <Uid>33c6fe3b-ec97-4b8b-90c3-c8e427426d94</Uid>
      <Name>Zvonok</Name>
      <DisplayName>Звонок</DisplayName>
      <Images />
      <NamedImages />
      <TypeUid>72ed98ca-f260-4671-9bcd-ff1d80235f47</TypeUid>
      <SubTypeUid>ef96d819-d015-4516-b39e-0a8a3c4e337d</SubTypeUid>
      <Settings xsi:type="EntitySettings">
        <FieldName>Zvonok</FieldName>
        <CascadeMode>SaveUpdate</CascadeMode>
      </Settings>
      <Nullable>true</Nullable>
      <OwnerPropertyUid>00000000-0000-0000-0000-000000000000</OwnerPropertyUid>
      <ViewSettings>
        <Attributes>
          <ViewAttribute>
            <ViewType>Create</ViewType>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Edit</ViewType>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Display</ViewType>
            <ReadOnly>true</ReadOnly>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>List</ViewType>
            <Visibility>Hidden</Visibility>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Filter</ViewType>
          </ViewAttribute>
        </Attributes>
      </ViewSettings>
      <Order>2</Order>
      <Handlers />
      <TypeResolver>00000000-0000-0000-0000-000000000000</TypeResolver>
    </PropertyMetadata>
    <PropertyMetadata xsi:type="EntityPropertyMetadata">
      <Uid>65881ed6-a68e-4267-9e87-d203426a9187</Uid>
      <Name>SsylkaNaZapisj</Name>
      <DisplayName>Ссылка на запись</DisplayName>
      <Images />
      <NamedImages />
      <TypeUid>66a64042-0490-4ad6-b804-4d08fa6c85f0</TypeUid>
      <SubTypeUid>00000000-0000-0000-0000-000000000000</SubTypeUid>
      <Settings xsi:type="UrlSettings">
        <FieldName>SsylkaNaZapisj</FieldName>
      </Settings>
      <Nullable>true</Nullable>
      <OwnerPropertyUid>00000000-0000-0000-0000-000000000000</OwnerPropertyUid>
      <ViewSettings>
        <Attributes>
          <ViewAttribute>
            <ViewType>Create</ViewType>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Edit</ViewType>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Display</ViewType>
            <ReadOnly>true</ReadOnly>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>List</ViewType>
            <Visibility>Hidden</Visibility>
          </ViewAttribute>
          <ViewAttribute>
            <ViewType>Filter</ViewType>
          </ViewAttribute>
        </Attributes>
      </ViewSettings>
      <Order>3</Order>
      <Handlers />
      <TypeResolver>00000000-0000-0000-0000-000000000000</TypeResolver>
    </PropertyMetadata>
  </Properties>
  <DefaultForms>
    <CreateUid>00000000-0000-0000-0000-000000000000</CreateUid>
    <EditUid>00000000-0000-0000-0000-000000000000</EditUid>
    <DisplayUid>00000000-0000-0000-0000-000000000000</DisplayUid>
    <ActionGuids />
    <FormSettings />
  </DefaultForms>
  <Forms />
  <FormTransformations />
  <FormViews />
  <TableViews>
    <TableView>
      <Uid>ffe55844-d4de-4efc-be76-42e808c178a6</Uid>
      <ViewType>List</ViewType>
      <SortDescriptors />
      <GroupDescriptors />
    </TableView>
  </TableViews>
  <TitlePropertyUid>00000000-0000-0000-0000-000000000000</TitlePropertyUid>
  <Type>Interface</Type>
  <ImplementationUid>fe1e73df-7551-45ef-8136-276428ff4bb0</ImplementationUid>
  <IdTypeUid>d90a59af-7e47-48c5-8c4c-dad04834e6e3</IdTypeUid>
  <TableName>CallRecord</TableName>
  <IsSoftDeletable>true</IsSoftDeletable>
  <ParentPropertyUid>00000000-0000-0000-0000-000000000000</ParentPropertyUid>
  <IsGroupPropertyUid>00000000-0000-0000-0000-000000000000</IsGroupPropertyUid>
  <Filter>
    <Uid>00000000-0000-0000-0000-000000000000</Uid>
    <Images />
    <NamedImages />
    <BaseClassUid>00000000-0000-0000-0000-000000000000</BaseClassUid>
    <Properties />
    <DefaultForms>
      <CreateUid>00000000-0000-0000-0000-000000000000</CreateUid>
      <EditUid>00000000-0000-0000-0000-000000000000</EditUid>
      <DisplayUid>00000000-0000-0000-0000-000000000000</DisplayUid>
      <ActionGuids />
      <FormSettings />
    </DefaultForms>
    <Forms />
    <FormTransformations />
    <FormViews />
    <TableViews />
    <TitlePropertyUid>00000000-0000-0000-0000-000000000000</TitlePropertyUid>
  </Filter>
  <ImplementedExtensionUids />
  <Actions>
    <Uid>00000000-0000-0000-0000-000000000000</Uid>
    <Images />
    <NamedImages />
    <BaseTypeUid>00000000-0000-0000-0000-000000000000</BaseTypeUid>
    <Values />
  </Actions>
  <TableParts />
</Entity>