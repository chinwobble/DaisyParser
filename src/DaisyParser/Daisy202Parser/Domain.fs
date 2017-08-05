namespace DaisyParser.Parser

open System
// model based on 2.02 standard 
// http://www.daisy.org/z3986/specifications/daisy_202.html?q=publications/specifications/daisy_202.html#textdoc
module Domain =

  type MultiMediaType = 
  | ``Full Audio With Title Only`` // no navigation 
  | ``Full Audio With NCC Only``
  | ``Full Audio with NCC and Partial Text`` 
  | ``Full Audio with some text``
  | ``Full Audio and full text``
  | ``Full Text And no Audio`` 
  | ``Full Text with some Audio``

  type MetaDataWithScheme<'a,'b> = {
    Content : 'a
    Scheme : 'b
  }

  type OptionalMetaData =
  | Contributor // dc:contributor
  | Coverage // dc:coverage extent or scope of resources 
  | Description // dc:description
  | Relation //dc:relation reference to a related resource
  | Rights // dc:rights
  | Source of MetaDataWithScheme<string, string option> // dc:source
  | Subject of MetaDataWithScheme<string, string option> // dc:subject
  | Type of MetaDataWithScheme<string, string option> // dc:type 
  | Depth of int // ncc:depth
  | Files of int // ncc:files
  | Footnotes of int //ncc:footnotes
  | Generator of string // ncc:generator content: name and version of software that generated the NCC document
  | KByteSize of double //ncc:kByteSize
  | PageSize of int // ncc:maxPageNormal positive integer or zero indicating the content of the highest normal page occuring in the DTB
  | MultimediaType of MultiMediaType // ncc:multimediaType one of the DAISY DTB categories listed in section 1.3 above: type 1 "audioOnly"; type 2 "audioNcc"; type 3 "audioPartText"; type 4 "audioFullText"; type 5 "textPartAudio"; type 6 "textNcc". (See also the DAISY structure guidelines)
  | Narrator of string // ncc:narrator
  | ProdNotes of int // ncc:prodNotes mandatory if producer notes are used 
  | ProducedDate of MetaDataWithScheme<string, string option> //  ncc:producedDate  content: date of first generation of DTB
  | Revision of int  // ncc:revision content: integer describing revision number
  | RevisionDate of MetaDataWithScheme<string, string option> //ncc:revisionDate
  | SetInfo of int //ncc:setInfo
//content: k of n ; for the kth medium of a DTB spanned on n distribution media
//occurrence: mandatory in multiple volume DTB´s; inclusion in single volume DTB´s is optional - recommended
//note: playback systems must also accept elements using the deprecated name "ncc:setinfo"
  | SideBars of int 
  | SourceDate of DateTime
  | SourceEdition of string 
  | SourcePublisher of string 
  | SourceRights of string 
  | SourceTitle of string 
  | HttpEquiv of string // http-equiv

  type MetaData = {
    Creator : string // dc:creator
    Date : DateTime //dc:date
    Format : string // dc:format should be Daisy 2.02
    //Identifier : MetaDataWithScheme<string, string option> //dc:identifier
    //Language : MetaDataWithScheme<string, string option> //schemes: ISO 639 language code optionally followed by a two letter country code as in ISO 3166. For Sweden: "SV" or "SV-SE", for the United Kingdom: "EN" or "EN-UK" etc.
    Publisher : string //dc:publisher
    Title : string //dc:title
    Charset : string // ncc:charset
    PageFront : int // ncc:pageFront or ncc:page-front
    PageNormal : int // ncc:pageNormal or ncc:page-normal total number of normal pages
    PageSpecial : int // ncc:prodNotes
    TocItems : int // ncc:tocItems or ncc:tocitems or ncc:TOCitems
    TotalTime : TimeSpan // ncc:totalTime

    OptionalMetaData : OptionalMetaData seq
    
  }

  type NavigationControlCentre = {
    Title : string
    Meta : MetaData }
    