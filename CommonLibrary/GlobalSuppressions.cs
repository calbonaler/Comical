// このファイルは、このプロジェクトに適用される SuppressMessage 
//属性を保持するために、コード分析によって使用されます。
// プロジェクト レベルの抑制には、ターゲットがないものと、特定のターゲット
//が指定され、名前空間、型、メンバーなどをスコープとするものがあります。
//
// このファイルに抑制を追加するには、[コード分析] の結果でメッセージを 
// 右クリックし、[メッセージの非表示] をポイントして、
// [抑制ファイル内] をクリックします。
// このファイルに手動で抑制を追加する必要はありません。

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "CommonLibrary.CommonUtils.#ExtendFrameIntoClientArea(System.Windows.Forms.Form,System.Boolean)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", MessageId = "0", Scope = "member", Target = "CommonLibrary.CommonUtils.#SendIfNeeded(System.Threading.SynchronizationContext,System.Action)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", Scope = "member", Target = "CommonLibrary.NativeMethods.#GetClassLongPtr64(System.IntPtr,CommonLibrary.WindowClassMember)")]
